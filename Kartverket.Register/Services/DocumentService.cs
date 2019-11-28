using System;
using System.Linq;
using Kartverket.Register.Models;
using System.Data.Entity;
using System.Web;
using Kartverket.Register.Services.Versioning;

namespace Kartverket.Register.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly RegisterDbContext _dbContext;
        private readonly IVersioningService _versioningService;

        public DocumentService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
            _versioningService = new VersioningService(_dbContext);
        }


        public Document UpdateDocument(Document originalDocument, Document document, HttpPostedFileBase documentfile, HttpPostedFileBase thumbnail, bool retired, bool sosi)
        {
            originalDocument.Update(document);
            originalDocument = ApprovalProcess(originalDocument, document, retired, sosi);
            _dbContext.SaveChanges();

            return originalDocument;
        }

        private Document ApprovalProcess(Document originalDocument, Document document, bool retired, bool sosi)
        {
            if (document.IsAccepted())
            {
                originalDocument = SetStatusIdWhenOriginalDocumentIsAccepted(originalDocument, document, retired, sosi);
            }
            else if (document.IsNotAccepted())
            {
                if (originalDocument.IsCurrentVersion()) // IsCurrentVersion
                {
                    UpdateCurrentVersion(originalDocument);
                }
                originalDocument.statusId = GetStatusIdWhenDocumentIsNotAccepted(originalDocument, retired, document);
            }
            return originalDocument;
        }

        private string GetStatusIdWhenDocumentIsNotAccepted(Document originalDocument, bool retired, Document document)
        {
            if (retired)
            {
                originalDocument.statusId = "Retired";
                originalDocument.DateRetired = DateTime.Now;
            }
            else
            {
                originalDocument.statusId = "Draft";
                originalDocument.dateNotAccepted = DateTime.Now;
            }
            return originalDocument.statusId;
        }

        private void UpdateCurrentVersion(Document originalDocument)
        {
            var newCurrentVersion = _versioningService.GetLatestSupersededVersion(originalDocument.versioningId);

            if (newCurrentVersion == null)
            {
                newCurrentVersion = _versioningService.SetLatestDocumentWithStatusIdDraftAsCurrent(originalDocument.versioningId);
                if (newCurrentVersion == null)
                {
                    newCurrentVersion = _versioningService.SetLatestDocumentWithStatusIdSubmittedAsCurrent(originalDocument.versioningId);
                    if (newCurrentVersion == null)
                    {
                        newCurrentVersion = _versioningService.SetLatestDocumentWithStatusIdRetiredAsCurrent(originalDocument.versioningId);
                    }
                }
            }
            else
            {
                newCurrentVersion.statusId = "Valid";
                newCurrentVersion.modified = DateTime.Now;
                newCurrentVersion.dateSuperseded = null;
            }

            if (newCurrentVersion == null)
                newCurrentVersion = originalDocument;

            _versioningService.UpdateCurrentVersionOfVersionGroup(newCurrentVersion.versioningId, newCurrentVersion.systemId);

            _dbContext.Entry(originalDocument.versioning).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        private Document SetStatusIdWhenOriginalDocumentIsAccepted(Document originalDocument, Document document, bool retired, bool sosi)
        {
            if (retired || originalDocument.IsRetired())
            {
                originalDocument.SetDocumentAsRetired();

                //Update current version...
                if (originalDocument.IsCurrentVersion())
                {
                    var latestSupersededVersion = _versioningService.GetLatestSupersededVersion(originalDocument.versioningId);
                    if (latestSupersededVersion != null)
                    {
                        latestSupersededVersion.statusId = SetStatusToValid(sosi);
                        UpdateCurrentVersion(latestSupersededVersion);
                    }
                }
            }
            else
            {
                originalDocument.dateAccepted = document.GetDateAccepted();
                if (IsOtherVersions(originalDocument))
                {
                    if (originalDocument.IsCurrentVersion())
                    {
                        var latestSupersededVersion = _versioningService.GetLatestSupersededVersion(originalDocument.versioningId);
                        if (latestSupersededVersion != null)
                        {
                            SetLatesAcceptedVersionAsCurrent(originalDocument, sosi, latestSupersededVersion);
                        }
                        else
                        {
                            originalDocument.statusId = SetStatusToValid(sosi);
                        }
                    }
                    else
                    {
                        var currentVersion = GetDocumentById(originalDocument.versioning.currentVersion);

                        if (OriginalDocumentDateAcceptedAreLatest(originalDocument.dateAccepted, currentVersion.dateAccepted))
                        {
                            originalDocument.statusId = SetStatusToValid(sosi);
                            currentVersion.SetStatusSuperseded();
                            currentVersion.modified = DateTime.Now;
                            _versioningService.UpdateCurrentVersionOfVersionGroup(originalDocument.versioningId, originalDocument.systemId);
                        }
                        else
                        {
                            originalDocument.SetStatusSuperseded();
                        }
                    }
                }
                else
                {
                    originalDocument.statusId = SetStatusToValid(sosi);
                }
            }
            _dbContext.SaveChanges();

            return originalDocument;
        }

        private void SetLatesAcceptedVersionAsCurrent(Document originalDocument, bool sosi, Document latestSupersededVersion)
        {
            if (OriginalDocumentDateAcceptedAreLatest(originalDocument.dateAccepted, latestSupersededVersion.dateAccepted))
            {
                originalDocument.statusId = SetStatusToValid(sosi);
            }
            else
            {
                latestSupersededVersion.statusId = SetStatusToValid(sosi);
                originalDocument.SetStatusSuperseded();
                UpdateCurrentVersion(latestSupersededVersion);
            }
        }

        private bool OriginalDocumentDateAcceptedAreLatest(DateTime? originalDocumentDateAccepted, DateTime? currentVersionDateAccepted)
        {
            return currentVersionDateAccepted == null || currentVersionDateAccepted < originalDocumentDateAccepted || currentVersionDateAccepted == originalDocumentDateAccepted;
        }

        private string SetStatusToValid(bool sosi)
        {
            return sosi ? "Sosi-valid" : "Valid";
        }

        private bool IsOtherVersions(Document originalDocument)
        {
            var versions = _versioningService.GetVersionsByVersioningId(originalDocument.versioningId);
            return versions.Count > 1;
        }

        private Document GetDocumentById(Guid systemId)
        {
            var queryResult = from d in _dbContext.Documents
                              where d.systemId == systemId
                              select d;

            return queryResult.FirstOrDefault();
        }
    }
}