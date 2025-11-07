using CommunityToolkit.Mvvm.ComponentModel;

namespace BusinessSharkClient.Logic.ViewModels
{
    public partial class SawmillDetailViewModel : ObservableObject
    {
        private SawmillProvider _sawmillProvider;
        private GlobalDataProvider _globalDataProvider;

        public SawmillDetailViewModel(GlobalDataProvider globalDataProvider, SawmillProvider sawmillProvider) 
        { 
            _sawmillProvider = sawmillProvider;
            _globalDataProvider = globalDataProvider;
        }

        public int DivisionId { get; set; }

        [ObservableProperty]
        private ImageSource? productIcon;

        [ObservableProperty]
        private string? name;

        [ObservableProperty]
        private string? description;

        [ObservableProperty]
        private string? city;

        [ObservableProperty]
        private string? countryFlag;

        [ObservableProperty]
        private int volumeCapacity;

        [ObservableProperty]
        private double techLevel;

        [ObservableProperty]
        private double resourceDepositQuality;


        [ObservableProperty]
        private int lastProductionQuantity;

        [ObservableProperty]
        private double lastProductionQuality;

        [ObservableProperty]
        private double balance;

        [ObservableProperty]
        private double quantityBonus;

        [ObservableProperty]
        private double qualityBonus;

        [ObservableProperty]
        private int workerCount = 45;

        [ObservableProperty]
        private double qualification = 0.76;

        [ObservableProperty]
        private double salary = 38000;

        [ObservableProperty]
        private double trainingProgress = 0.15;

        [ObservableProperty]
        private int equipmentCount = 15;

        [ObservableProperty]
        private double equipmentTechLevel = 0.82;

        [ObservableProperty]
        private double wear = 0.12;


        internal async Task LoadAsync(int divisionId)
        {
            var response = await _sawmillProvider.LoadDetail(divisionId);

            // Cache product definitions for fast lookup
            var productDefinition = _globalDataProvider.ProductDefinitions.FirstOrDefault(p => p.ProductDefinitionId == response.ProductDefinitionId);

            // SAWMILL
            DivisionId = divisionId;
            
            Name = response.Name;
            Description = response.Description;
            City = response.City;
            CountryFlag = $"{response.CountryCode.ToLowerInvariant()}.png";
            VolumeCapacity = response.VolumeCapacity;
            TechLevel = response.TechLevel;
            ResourceDepositQuality = response.ResourceDepositQuality;

            if (productDefinition != null)
            {
                ProductIcon = productDefinition.Image;
            }

            // PRODUCTION
            QualityBonus = response.QualityBonus;
            QuantityBonus = response.QuantityBonus;

            var divisionStatistics = DivisionTransactionViewModel.GetLastTransaction(response.DivisionTransactions.ToList());
            Balance = divisionStatistics.Balance;
            LastProductionQuantity = divisionStatistics.QuantityProduced;
            LastProductionQuality = divisionStatistics.QualityProduced;

            // Employee
            WorkerCount = response.Employees.TotalQuantity;
            Qualification = response.Employees.SkillLevel;
            Salary = response.Employees.SalaryPerEmployee;
            TrainingProgress = 0;

            // Equipment
            EquipmentCount = response.Tools.TotalQuantity;
            EquipmentTechLevel = response.Tools.TechLevel;
            Wear = response.Tools.Deprecation;
        }

    }
}
