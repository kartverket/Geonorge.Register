﻿using Eu.Europa.Ec.Jrc.Inspire;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class InspireMonitoring
    {
        private ICollection<InspireDataset> _inspireDataset;
        private ICollection<InspireDataService> _inspireDataService;
        private readonly string Download = "download";
        private readonly string View = "view";
        private readonly string Discovery = "discovery";
        private readonly string Invoke = "invoke";
        private readonly string Transformation = "transformation";


        public Guid Id { get; set; }
        public System.DateTime Date { get; set; }

        public int NumberOfDatasetsByAnnexI { get; set; }
        public int NumberOfDatasetsByAnnexII { get; set; }
        public int NumberOfDatasetsByAnnexIII { get; set; }

        public int NumberOfDatasetsByAnnexIWithMetadata { get; set; }
        public int NumberOfDatasetsByAnnexIIWithMetadata { get; set; }
        public int NumberOfDatasetsByAnnexIIIWithMetadata { get; set; }

        public int NumberOfDatasetsByAnnexIWhereMetadataStatusIsgood { get; set; }
        public int NumberOfDatasetsByAnnexIIWhereMetadataStatusIsgood { get; set; }
        public int NumberOfDatasetsByAnnexIIIWhereMetadataStatusIsgood { get; set; }
        public int NumberOfServicesWhereMetadataStatusIsgood { get; set; }

        public int NumberOfDatasetsWithMetadata { get; set; }
        public int NumberOfServicesWithMetadata { get; set; }

        public int NumberOfDatasetsRegisteredInADiscoveryService { get; set; }
        public int NumberOfServicesRegisteredInADiscoveryService { get; set; }

        public int NumberOfServicesByServiceTypeDownload { get; set; }
        public int NumberOfServicesByServiceTypeView { get; set; }
        public int NumberOfServicesByServiceTypeDiscovery { get; set; }
        public int NumberOfServicesByServiceTypeInvoke { get; set; }
        public int NumberOfServicesByServiceTypeTransformation { get; set; }
        public int NumberOfSdS { get; set; }

        public int NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue { get; set; }
        public int NumberOfServicesByServiceTypeViewWhereConformityIsTrue { get; set; }
        public int NumberOfServicesByServiceTypeDiscoveryWhereConformityIsTrue { get; set; }
        public int NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue { get; set; }
        public int NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue { get; set; }

        public int NumberOfCallsByServiceTypeDiscovery { get; set; }

        public int NumberOfCallsByServiceTypeView { get; set; }
        public int NumberOfCallsByServiceTypeDownload { get; set; }
        public int NumberOfCallsByServiceTypeTransformation { get; set; }
        public int NumberOfCallsByServiceTypeInvoke { get; set; }

        public int NumberOfDatasetsAvailableThroughViewANDDownloadService { get; set; }
        public int NumberOfDatasetsAvailableThroughDownloadService { get; set; }
        public int NumberOfDatasetsAvailableThroughViewService { get; set; }

        public int NumberOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata { get; set; }
        public int NumberOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata { get; set; }
        public int NumberOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata { get; set; }

        public double AccumulatedCurrentAreaByAnnexI { get; set; }
        public double AccumulatedCurrentAreaByAnnexII { get; set; }
        public double AccumulatedCurrentAreaByAnnexIII { get; set; }

        public double AccumulatedRelevantAreaByAnnexI { get; set; }
        public double AccumulatedRelevantAreaByAnnexII { get; set; }
        public double AccumulatedRelevantAreaByAnnexIII { get; set; }

        public InspireMonitoring(ICollection<RegisterItemV2> inspireItems)
        {
            _inspireDataset = GetInspireDatasets(inspireItems);
            _inspireDataService = GetInspireDataService(inspireItems);

            NumberOfDatasetsByAnnexI = GetNumberOfDatasetsByAnnexI();
            NumberOfDatasetsByAnnexII = GetNumberOfDatasetsByAnnexII();
            NumberOfDatasetsByAnnexIII = GetNumberOfDatasetsByAnnexIII();

            NumberOfDatasetsByAnnexIWithMetadata = GetNumberOfDatasetsByAnnexIWithMetadata();
            NumberOfDatasetsByAnnexIIWithMetadata = GetNumberOfDatasetsByAnnexIIWithMetadata();
            NumberOfDatasetsByAnnexIIIWithMetadata = GetNumberOfDatasetsByAnnexIIIWithMetadata();

            NumberOfDatasetsByAnnexIWhereMetadataStatusIsgood = GetNumberOfDatasetsByAnnexIWhereMetadataStatusIsgood();
            NumberOfDatasetsByAnnexIIWhereMetadataStatusIsgood = GetNumberOfDatasetsByAnnexIIWhereMetadataStatusIsgood();
            NumberOfDatasetsByAnnexIIIWhereMetadataStatusIsgood = GetNumberOfDatasetsByAnnexIIIWhereMetadataStatusIsgood();
            NumberOfServicesWhereMetadataStatusIsgood = GetNumberOfServicesWhereMetadataStatusIsgood();

            NumberOfDatasetsWithMetadata = GetNumberOfDatasetsWithMetadata();
            NumberOfServicesWithMetadata = GetNumberOfServicesWithMetadata();

            NumberOfServicesRegisteredInADiscoveryService = GetNumberOfServicesRegisteredInADiscoveryService();
            NumberOfDatasetsRegisteredInADiscoveryService = GetNumberOfDatasetsRegisteredInADiscoveryService();

            NumberOfServicesByServiceTypeDownload = GetNumberOfServicesByServiceType(Download);
            NumberOfServicesByServiceTypeView = GetNumberOfServicesByServiceType(View);
            NumberOfServicesByServiceTypeDiscovery = GetNumberOfServicesByServiceType(Discovery);
            NumberOfServicesByServiceTypeInvoke = GetNumberOfServicesByServiceType(Invoke);
            NumberOfServicesByServiceTypeTransformation = GetNumberOfServicesByServiceType(Transformation);
            NumberOfSdS = GetNumberOfSdS();

            NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue = GetNumberOfServicesByServiceTypeWhereConformityIsTrue(Download);
            NumberOfServicesByServiceTypeViewWhereConformityIsTrue = GetNumberOfServicesByServiceTypeWhereConformityIsTrue(View);
            NumberOfServicesByServiceTypeDiscoveryWhereConformityIsTrue = GetNumberOfServicesByServiceTypeWhereConformityIsTrue(Discovery);
            NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue = GetNumberOfServicesByServiceTypeWhereConformityIsTrue(Invoke);
            NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue = GetNumberOfServicesByServiceTypeWhereConformityIsTrue(Transformation);

            NumberOfCallsByServiceTypeDiscovery = GetNumberOfCallsByServiceType(Discovery);
            NumberOfCallsByServiceTypeView = GetNumberOfCallsByServiceType(View);
            NumberOfCallsByServiceTypeDownload = GetNumberOfCallsByServiceType(Download);
            NumberOfCallsByServiceTypeTransformation = GetNumberOfCallsByServiceType(Transformation);
            NumberOfCallsByServiceTypeInvoke = GetNumberOfCallsByServiceType(Invoke);

            NumberOfDatasetsAvailableThroughViewANDDownloadService = GetNumberOfDatasetsAvailableThroughViewANDDownloadService();
            NumberOfDatasetsAvailableThroughDownloadService = GetNumberOfDatasetsAvailableThroughDownloadService();
            NumberOfDatasetsAvailableThroughViewService = GetNumberOfDatasetsAvailableThroughViewService();

            NumberOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata = GetNumberOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata();
            NumberOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata = GetNumberOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata();
            NumberOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata = GetNumberOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata();

            AccumulatedCurrentAreaByAnnexI = GetAccumulatedCurrentAreaByAnnexI();
            AccumulatedCurrentAreaByAnnexII = GetAccumulatedCurrentAreaByAnnexII();
            AccumulatedCurrentAreaByAnnexIII = GetAccumulatedCurrentAreaByAnnexIII();

            AccumulatedRelevantAreaByAnnexI = GetAccumulatedRelevantAreaByAnnexI();
            AccumulatedRelevantAreaByAnnexII = GetAccumulatedRelevantAreaByAnnexII();
            AccumulatedRelevantAreaByAnnexIII = GetAccumulatedRelevantAreaByAnnexIII();

        }



        // *** Public  methods***

        public int NumberOfCallsByServiceType()
        {
            return NumberOfCallsByServiceTypeDiscovery +
                NumberOfCallsByServiceTypeView +
                NumberOfCallsByServiceTypeDownload +
                NumberOfCallsByServiceTypeTransformation +
                NumberOfCallsByServiceTypeInvoke;
        }


        public int NumberOfServicesByServiceType()
        {
            return NumberOfServicesByServiceTypeDownload +
                    NumberOfServicesByServiceTypeView +
                    NumberOfServicesByServiceTypeDiscovery +
                    NumberOfServicesByServiceTypeInvoke +
                    NumberOfServicesByServiceTypeTransformation;
        }

        public int NumberOfDatasetsWithHarmonizedDataAndConformedMetadata()
        {
            return NumberOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata +
                   NumberOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata +
                   NumberOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata;
        }


        public int NumberOfServicesByServiceTypeWhereConformityIsTrue()
        {
            return NumberOfServicesByServiceTypeDiscoveryWhereConformityIsTrue +
                    NumberOfServicesByServiceTypeViewWhereConformityIsTrue +
                    NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue +
                    NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue +
                    NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue;
        }


        public int NumberOfDatasetsByAnnexWhereMetadataStatusIsgood()
        {
            return NumberOfDatasetsByAnnexIIWhereMetadataStatusIsgood +
                    NumberOfDatasetsByAnnexIIWhereMetadataStatusIsgood +
                    NumberOfDatasetsByAnnexIIIWhereMetadataStatusIsgood;
        }


        public double AccumulatedCurrentAreaByAnnex()
        {
            return AccumulatedCurrentAreaByAnnexI +
                    AccumulatedCurrentAreaByAnnexII +
                    AccumulatedCurrentAreaByAnnexIII;
        }


        public double AccumulatedRelevantAreaByAnnex()
        {
            return AccumulatedRelevantAreaByAnnexI +
                AccumulatedRelevantAreaByAnnexII +
                AccumulatedRelevantAreaByAnnexIII;
        }


        public double AverageNumberOfCallsByServiceTypeDownload()
        {
            return Divide(NumberOfCallsByServiceTypeDownload, NumberOfServicesByServiceTypeDownload);
        }

        public double AverageNumberOfCallsByServiceTypeDiscovery()
        {
            return Divide(NumberOfCallsByServiceTypeDiscovery, NumberOfServicesByServiceTypeDiscovery);
        }

        public double AverageNumberOfCallsByServiceTypeView()
        {
            return Divide(NumberOfCallsByServiceTypeView, NumberOfServicesByServiceTypeView);
        }

        public double AverageNumberOfCallsByServiceTypeTransformation()
        {
            return Divide(NumberOfCallsByServiceTypeTransformation, NumberOfServicesByServiceTypeTransformation);
        }

        public double AverageNumberOfCallsByServiceTypeInvoke()
        {
            return Divide(NumberOfCallsByServiceTypeInvoke, NumberOfServicesByServiceTypeInvoke);
        }

        public double AverageNumberOfCallsByServiceType()
        {
            return Divide(NumberOfCallsByServiceType(), NumberOfServicesByServiceType());
        }

        public double ProportionOfDatasetsRegisteredInADiscoveryService()
        {
            return Divide(NumberOfDatasetsRegisteredInADiscoveryService, NumberOfDatasetsByAnnex());
        }

        public double ProportionOfServicesRegisteredInADiscoveryService()
        {
            return Divide(NumberOfServicesRegisteredInADiscoveryService, NumberOfServicesByServiceType());
        }


        public double ProportionOfDatasetsWithMetadataByAnnexI()
        {
            return Divide(NumberOfDatasetsByAnnexIWithMetadata, NumberOfDatasetsByAnnexI);
        }

        public double ProportionOfDatasetsWithMetadataByAnnexII()
        {
            return Divide(NumberOfDatasetsByAnnexIIWithMetadata, NumberOfDatasetsByAnnexII);
        }

        public double ProportionOfDatasetsWithMetadataByAnnexIII()
        {
            return Divide(NumberOfDatasetsByAnnexIIIWithMetadata, NumberOfDatasetsByAnnexIII);
        }

        public double ProportionOfServicesWithMetadata()
        {
            return Divide(NumberOfServicesWithMetadata, NumberOfServicesByServiceType());
        }

        public double ProportionOfDatasetsWithMetadata()
        {
            return Divide(NumberOfDatasetsWithMetadata, NumberOfDatasetsByAnnex());
        }

        public int NumberOfDatasetsByAnnex()
        {
            return NumberOfDatasetsByAnnexI + NumberOfDatasetsByAnnexII + NumberOfDatasetsByAnnexIII;
        }

        public double ProportionOfDatasetWithHarmonizedDataAndConformedMetadata()
        {
            return Divide(NumberOfDatasetsWithHarmonizedDataAndConformedMetadata(), NumberOfDatasetsByAnnex());
        }

        public double ProportionOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata()
        {
            return Divide(NumberOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata, NumberOfDatasetsByAnnexI);
        }

        public double ProportionOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata()
        {
            return Divide(NumberOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata, NumberOfDatasetsByAnnexII);
        }

        public double ProportionOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata()
        {
            return Divide(NumberOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata, NumberOfDatasetsByAnnexIII);
        }

        public double ProportionOfDatasetsAvailableThroughViewService()
        {
            return Divide(NumberOfDatasetsAvailableThroughViewService, NumberOfServicesByServiceTypeView);
        }

        public double ProportionOfDatasetsAvailableThroughDownloadService()
        {
            return Divide(NumberOfDatasetsAvailableThroughDownloadService, NumberOfServicesByServiceTypeDownload);
        }

        public double ProportionOfDatasetsAvailableThroughViewAndDownloadService()
        {
            return Divide(NumberOfDatasetsAvailableThroughViewANDDownloadService, (NumberOfServicesByServiceTypeDownload + NumberOfServicesByServiceTypeView));
        }

        public double ProportionOfServicesWhereConformityIsTrue()
        {
            return Divide(NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue, NumberOfServicesByServiceType());
        }

        public double ProportionOfServicesByServiceTypeDownloadWhereConformityIsTrue()
        {
            return Divide(NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue, NumberOfServicesByServiceTypeDownload);
        }

        public double ProportionOfServicesByServiceTypeDiscoveryWhereConformityIsTrue()
        {
            return Divide(NumberOfServicesByServiceTypeDiscoveryWhereConformityIsTrue, NumberOfServicesByServiceTypeDiscovery);
        }

        public double ProportionOfServicesByServiceTypeViewWhereConformityIsTrue()
        {
            return Divide(NumberOfServicesByServiceTypeViewWhereConformityIsTrue, NumberOfServicesByServiceTypeView);
        }

        public double ProportionOfServicesByServiceTypeTransformationWhereConformityIsTrue()
        {
            return Divide(NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue, NumberOfServicesByServiceTypeTransformation);
        }

        public double ProportionOfServicesByServiceTypeInvokeWhereConformityIsTrue()
        {
            return Divide(NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue, NumberOfServicesByServiceTypeInvoke);
        }

        public double ProportionOfServicesAndDatasetsRegisteredInADiscoveryService()
        {
            return Divide((NumberOfDatasetsRegisteredInADiscoveryService + NumberOfServicesRegisteredInADiscoveryService), (NumberOfDatasetsByAnnex() + NumberOfServicesByServiceType()));
        }

        public double ProportionOfArealByAnnexI()
        {
            return Divide(AccumulatedCurrentAreaByAnnexI, AccumulatedRelevantAreaByAnnexI);
        }

        public double ProportionOfArealByAnnexII()
        {
            return Divide(AccumulatedCurrentAreaByAnnexII, AccumulatedRelevantAreaByAnnexII);
        }

        public double ProportionOfArealByAnnexIII()
        {
            return Divide(AccumulatedCurrentAreaByAnnexIII, AccumulatedRelevantAreaByAnnexIII);
        }

        public double ProportionOfArealByAnnex()
        {
            return Divide(AccumulatedCurrentAreaByAnnex(), AccumulatedRelevantAreaByAnnex());
        }

        public double ProportionOfDatasetByAnnexIWithMetadatastatusGood()
        {
            return Divide(NumberOfDatasetsByAnnexIWhereMetadataStatusIsgood, NumberOfDatasetsByAnnexI);
        }

        public double ProportionOfDatasetByAnnexIIWithMetadatastatusGood()
        {
            return Divide(NumberOfDatasetsByAnnexIIWhereMetadataStatusIsgood, NumberOfDatasetsByAnnexII);
        }

        public double ProportionOfDatasetByAnnexIIIWithMetadatastatusGood()
        {
            return Divide(NumberOfDatasetsByAnnexIIIWhereMetadataStatusIsgood, NumberOfDatasetsByAnnexIII);
        }

        public double ProportionOfServicesWithMetadatastatusGood()
        {
            return Divide(NumberOfServicesWhereMetadataStatusIsgood, NumberOfServicesByServiceType());
        }

        public double ProportionOfServicesAndDatasetsWithMetadatastatusGood()
        {
            return Divide(NumberOfServicesWhereMetadataStatusIsgood + NumberOfDatasetsByAnnexWhereMetadataStatusIsgood(), NumberOfServicesByServiceType() + NumberOfDatasetsByAnnex());
        }




        // ******** PRIVATE *********


        private int GetNumberOfDatasetsByAnnexI()
        {
            var number = 0;
            foreach (var item in _inspireDataset)
            {
                if (InspireDatasetHaveInspireThemeOfTypeAnnexI(item.InspireThemes))
                {
                    number++;
                }
            }
            return number;
        }

        private int GetNumberOfDatasetsByAnnexII()
        {
            var number = 0;
            foreach (var item in _inspireDataset)
            {
                if (InspireDatasetHaveInspireThemeOfTypeAnnexII(item.InspireThemes))
                {
                    number++;
                }
            }
            return number;
        }

        private int GetNumberOfDatasetsByAnnexIII()
        {
            var number = 0;
            foreach (var item in _inspireDataset)
            {
                if (InspireDatasetHaveInspireThemeOfTypeAnnexIII(item.InspireThemes))
                {
                    number++;
                }
            }
            return number;
        }


        private int GetNumberOfDatasetsByAnnexIWithMetadata()
        {
            var number = 0;

            foreach (var item in _inspireDataset)
            {
                if (InspireDatasetHaveInspireThemeOfTypeAnnexI(item.InspireThemes))
                {
                    if (item.MetadataIsGoodOrDeficent())
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        private int GetNumberOfDatasetsByAnnexIIWithMetadata()
        {
            var number = 0;

            foreach (var item in _inspireDataset)
            {
                if (InspireDatasetHaveInspireThemeOfTypeAnnexII(item.InspireThemes))
                {
                    if (item.MetadataIsGoodOrDeficent())
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        private int GetNumberOfDatasetsByAnnexIIIWithMetadata()
        {
            var number = 0;

            foreach (var item in _inspireDataset)
            {
                if (InspireDatasetHaveInspireThemeOfTypeAnnexIII(item.InspireThemes))
                {
                    if (item.MetadataIsGoodOrDeficent())
                    {
                        number++;
                    }
                }
            }
            return number;
        }


        private int GetNumberOfDatasetsByAnnexIWhereMetadataStatusIsgood()
        {
            int number = 0;

            foreach (var item in _inspireDataset)
            {
                if (InspireDatasetHaveInspireThemeOfTypeAnnexI(item.InspireThemes))
                {
                    if (item.MetadataIsGood())
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        private int GetNumberOfDatasetsByAnnexIIWhereMetadataStatusIsgood()
        {
            int number = 0;

            foreach (var item in _inspireDataset)
            {
                if (InspireDatasetHaveInspireThemeOfTypeAnnexII(item.InspireThemes))
                {
                    if (item.MetadataIsGood())
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        private int GetNumberOfDatasetsByAnnexIIIWhereMetadataStatusIsgood()
        {
            int number = 0;

            foreach (var item in _inspireDataset)
            {
                if (InspireDatasetHaveInspireThemeOfTypeAnnexIII(item.InspireThemes))
                {
                    if (item.MetadataIsGood())
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        private int GetNumberOfServicesWhereMetadataStatusIsgood()
        {
            int number = 0;

            foreach (var item in _inspireDataService)
            {
                if (item.MetadataIsGood())
                {
                    number++;
                }
            }
            return number;
        }


        private int GetNumberOfServicesWithMetadata()
        {
            var number = 0;

            foreach (var item in _inspireDataService)
            {
                if (item.MetadataIsGoodOrDeficent())
                {
                    number++;
                }
            }
            return number;
        }

        private int GetNumberOfDatasetsWithMetadata()
        {
            var number = 0;

            foreach (var item in _inspireDataset)
            {
                if (item.MetadataIsGoodOrDeficent())
                {
                    number++;
                }
            }
            return number;
        }


        private int GetNumberOfDatasetsRegisteredInADiscoveryService()
        {
            int number = 0;

            foreach (var item in _inspireDataset)
            {
                number++;
            }
            return number;
        }

        private int GetNumberOfServicesRegisteredInADiscoveryService()
        {
            int number = 0;

            foreach (var item in _inspireDataService)
            {
                number++;
            }
            return number;
        }

        private int GetNumberOfServicesByServiceType(string serviceType)
        {
            int number = 0;
            foreach (var item in _inspireDataService)
            {
                if (item.ServiceType == serviceType)
                {
                    number++;
                }
            }
            return number;
        }

        private int GetNumberOfSdS()
        {
            var number = 0;
            foreach (var item in _inspireDataService)
            {
                if (item.IsSds())
                {
                    number++;
                }
            }
            return number;
        }


        private int GetNumberOfServicesByServiceTypeWhereConformityIsTrue(string serviceType)
        {
            int number = 0;
            foreach (var item in _inspireDataService)
            {
                if (item.InspireDeliveryServiceStatus.IsGood())
                {
                    if (string.IsNullOrWhiteSpace(serviceType))
                    {
                        number++;
                    }
                    else
                    {
                        if (item.ServiceType == serviceType)
                        {
                            number++;
                        }
                    }
                }
            }
            return number;
        }

        private int GetNumberOfCallsByServiceType(string serviceType)
        {
            int number = 0;
            foreach (var item in _inspireDataService)
            {
                if (item.ServiceType == serviceType)
                {
                    number += item.Requests;
                }
            }
            return number;
        }


        private int GetNumberOfDatasetsAvailableThroughViewANDDownloadService()
        {
            var number = 0;
            foreach (var item in _inspireDataset)
            {
                if (item.WmsAndWfsIsGoodOrUseable())
                {
                    number++;
                }
            }
            return number;
        }

        private int GetNumberOfDatasetsAvailableThroughDownloadService()
        {
            var number = 0;
            foreach (var item in _inspireDataset)
            {
                if (item.WfsIsGoodOrUseable())
                {
                    number++;
                }
            }
            return number;
        }

        private int GetNumberOfDatasetsAvailableThroughViewService()
        {
            var number = 0;
            foreach (var item in _inspireDataset)
            {
                if (item.WmsIsGoodOrUseable())
                {
                    number++;
                }
            }
            return number;
        }


        private int GetNumberOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata()
        {
            var number = 0;
            foreach (var item in _inspireDataset)
            {
                if (InspireDatasetHaveInspireThemeOfTypeAnnexI(item.InspireThemes))
                {
                    if (item.HarmonizedDataAndConformedmetadata())
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        private int GetNumberOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata()
        {
            var number = 0;
            foreach (var item in _inspireDataset)
            {
                if (InspireDatasetHaveInspireThemeOfTypeAnnexII(item.InspireThemes))
                {
                    if (item.HarmonizedDataAndConformedmetadata())
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        private int GetNumberOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata()
        {
            var number = 0;
            foreach (var item in _inspireDataset)
            {
                if (InspireDatasetHaveInspireThemeOfTypeAnnexIII(item.InspireThemes))
                {
                    if (item.HarmonizedDataAndConformedmetadata())
                    {
                        number++;
                    }
                }
            }
            return number;
        }


        private double GetAccumulatedCurrentAreaByAnnexI()
        {
            var number = 0;
            foreach (var item in _inspireDataset)
            {
                if (InspireDatasetHaveInspireThemeOfTypeAnnexI(item.InspireThemes))
                {
                    number += item.Area;
                }
            }
            return number;
        }

        private double GetAccumulatedCurrentAreaByAnnexII()
        {
            var number = 0;
            foreach (var item in _inspireDataset)
            {
                if (InspireDatasetHaveInspireThemeOfTypeAnnexII(item.InspireThemes))
                {
                    number += item.Area;
                }
            }
            return number;
        }

        private double GetAccumulatedCurrentAreaByAnnexIII()
        {
            var number = 0;
            foreach (var item in _inspireDataset)
            {
                if (InspireDatasetHaveInspireThemeOfTypeAnnexIII(item.InspireThemes))
                {
                    number += item.Area;
                }
            }
            return number;
        }


        private double GetAccumulatedRelevantAreaByAnnexI()
        {
            var number = 0;
            foreach (var item in _inspireDataset)
            {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexI(item.InspireThemes))
                    {
                        number += item.RelevantArea;
                    }
            }
            return number;
        }

        private double GetAccumulatedRelevantAreaByAnnexII()
        {
            var number = 0;
            foreach (var item in _inspireDataset)
            {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexII(item.InspireThemes))
                    {
                        number += item.RelevantArea;
                    }
            }
            return number;
        }

        private double GetAccumulatedRelevantAreaByAnnexIII()
        {
            var number = 0;
            foreach (var item in _inspireDataset)
            {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexIII(item.InspireThemes))
                    {
                        number += item.RelevantArea;
                    }
            }
            return number;
        }





       



        private ICollection<InspireDataService> GetInspireDataService(ICollection<RegisterItemV2> inspireItems)
        {
            var inspireDataServices = new List<InspireDataService>();
            foreach (var item in inspireItems)
            {
                if (item is InspireDataService inspireDataService)
                {
                    inspireDataServices.Add(inspireDataService);
                }
            }
            return inspireDataServices;
        }

        private ICollection<InspireDataset> GetInspireDatasets(ICollection<RegisterItemV2> inspireItems)
        {
            var inspireDatasets = new List<InspireDataset>();
            foreach (var item in inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    inspireDatasets.Add(inspireDataset);
                }
            }
            return inspireDatasets;
        }

        private bool InspireDatasetHaveInspireThemeOfTypeAnnexI(ICollection<CodelistValue> inspireThems)
        {
            try
            {
                foreach (var inspireTheme in inspireThems)
                {
                    if (IsAnnexI(inspireTheme))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        private bool InspireDatasetHaveInspireThemeOfTypeAnnexII(ICollection<CodelistValue> inspireThems)
        {
            foreach (var inspireTheme in inspireThems)
            {
                if (IsAnnexII(inspireTheme))
                {
                    return true;
                }
            }
            return false;
        }

        private bool InspireDatasetHaveInspireThemeOfTypeAnnexIII(ICollection<CodelistValue> inspireThems)
        {
            foreach (var inspireTheme in inspireThems)
            {
                if (IsAnnexIII(inspireTheme))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsAnnexI(CodelistValue inspireTheme)
        {
            if (inspireTheme != null)
            {
                var inspireThemeValueCamelCase = CreateCamelCase(inspireTheme.value);
                return Enum.IsDefined(typeof(AnnexI), inspireThemeValueCamelCase);
            }
            return false;
        }

        private static bool IsAnnexII(CodelistValue inspireTheme)
        {
            if (inspireTheme != null)
            {
                var inspireThemeValueCamelCase = CreateCamelCase(inspireTheme.value);
                return Enum.IsDefined(typeof(AnnexII), inspireThemeValueCamelCase);
            }
            return false;
        }

        private static bool IsAnnexIII(CodelistValue inspireTheme)
        {
            if (inspireTheme != null)
            {
                var inspireThemeValueCamelCase = CreateCamelCase(inspireTheme.value);
                return Enum.IsDefined(typeof(AnnexIII), inspireThemeValueCamelCase);
            }
            return false;
        }

        private static string CreateCamelCase(string inspireTheme)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            var inspireThemeCamelCase = textInfo.ToTitleCase(inspireTheme);
            inspireThemeCamelCase = Char.ToLowerInvariant(inspireThemeCamelCase[0]) + inspireThemeCamelCase.Substring(1);
            inspireThemeCamelCase = inspireThemeCamelCase.Replace(" ", "");

            return inspireThemeCamelCase;
        }

        private double Divide(int x, int y)
        {
            try
            {
                if (y == 0)
                {
                    return 0;
                }
                else
                {
                    return (double)x / y;
                }
            }
            catch (Exception e)
            {

                return 0;
            }
        }

        private double Divide(double x, double y)
        {
            try
            {
                if (y == 0)
                {
                    return 0;
                }
                else
                {
                    return x / y;
                }
            }
            catch (Exception e)
            {

                return 0;
            }
        }

    }
}