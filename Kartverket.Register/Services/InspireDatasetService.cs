using System.Linq;
using Kartverket.Register.Models;
using System.Collections.Generic;
using System;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Helpers;

namespace Kartverket.Register.Services
{
    public class InspireDatasetService : IInspireDatasetService
    {
        private readonly RegisterDbContext _dbContext;
        private IRegisterService _registerService;


        public InspireDatasetService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public InspireDatasetService()
        {
            _registerService = new RegisterService(_dbContext);
        }

        public void CreateNewInspireDataset(InspireDataset inspireDataset, string parentregister, string registername)
        {
            inspireDataset.Register = _registerService.GetRegister(parentregister, registername);
            inspireDataset.Seoname = RegisterUrls.MakeSeoFriendlyString(inspireDataset.Name);
            inspireDataset.SubmitterId = _registerService.GetOrganizationIdByUserName();
            inspireDataset.DateSubmitted = DateTime.Now;
            inspireDataset.Modified = DateTime.Now;
            //inspireDataset.StatusId = "Submitted";
            //inspireDataset.SystemId = Guid.NewGuid();
            _dbContext.InspireDatasets.Add(inspireDataset);
            _dbContext.SaveChanges();
        }
    }
}