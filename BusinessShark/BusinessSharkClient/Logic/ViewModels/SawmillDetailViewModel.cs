using System.Collections.ObjectModel;
using System.Windows.Input;
using BusinessSharkClient.Logic.Models;
using BusinessSharkShared;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BusinessSharkClient.Logic.ViewModels
{
    public partial class SawmillDetailViewModel : ObservableObject
    {
        public ICommand SaveDivisionDetailCommand { get; }
        private readonly SawmillProvider _sawmillProvider;
        private readonly GlobalDataProvider _globalDataProvider;

        public SawmillDetailViewModel(GlobalDataProvider globalDataProvider, SawmillProvider sawmillProvider, DivisionSizeProvider divisionSizeProvider) 
        {
            _sawmillProvider = sawmillProvider;
            _globalDataProvider = globalDataProvider;
            SaveDivisionDetailCommand = new Command(OnSaveDivisionDetail);
            SizeViewModel = new(divisionSizeProvider);

        }

        private void OnSaveDivisionDetail(object obj)
        {
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
        private int workerCount;

        [ObservableProperty]
        private double qualification;

        [ObservableProperty]
        private double salary;

        [ObservableProperty]
        private double trainingProgress;

        [ObservableProperty]
        private int equipmentCount;

        [ObservableProperty]
        private double equipmentTechLevel;

        [ObservableProperty]
        private double wear;

        [ObservableProperty]
        private double efficiency;

        public ObservableCollection<DivisionSizeModel> Sizes { get; set; } = new();


        public DivisionSizeViewModel SizeViewModel { get; }


        internal async Task LoadAsync(int divisionId)
        {
            var response = await _sawmillProvider.LoadDetail(divisionId);

            await SizeViewModel.LoadAsync(DivisionType.Sawmill);
            foreach (var size in SizeViewModel.Sizes)
            {
                Sizes.Add(size);
            }

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
            Wear = response.Tools.WearCoefficient;
            Efficiency = response.Tools.Efficiency;
        }

    }
}
