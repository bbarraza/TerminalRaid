using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TerminalRaid.Models;
    public class PlayerSpecs
    {
        [BsonElement("cpuTier")]
        public int CpuTier { get; set; }

        [BsonElement("ramTier")]
        public int RamTier { get; set; }

        [BsonElement("gpuTier")]
        public int GpuTier { get; set; }

        [BsonElement("diskTier")]
        public int DiskTier { get; set; }

        [BsonElement("psuTier")]
        public int PsuTier { get; set; }

        [BsonElement("lastUpgrade")]
        public DateTimeOffset LastUpgrade { get; set; }

        [BsonElement("totalSpent")]
        public double TotalSpent { get; set; }

        public PlayerSpecs()
        {
            CpuTier = 1;
            RamTier = 1;
            GpuTier = 1;
            DiskTier = 1;
            PsuTier = 1;
            LastUpgrade = DateTimeOffset.UtcNow;
            TotalSpent = 0;
        }

        public bool CanUpgrade(string componentType, int targetTier)
        {
            // Validate tier range (1-7 based on HardwareComponents)
            if (targetTier < 1 || targetTier > 7) return false;
            
            var currentTier = GetComponentTier(componentType);
            if (targetTier <= currentTier) return false;

            // PSU requirement check - PSU must be >= other components
            if (componentType != "PSU")
            {
                var requiredPsuTier = Math.Max(PsuTier, targetTier);
                if (PsuTier < requiredPsuTier) return false;
            }

            return true;
        }

        public double GetUpgradeCost(string componentType, int targetTier)
        {
            if (!CanUpgrade(componentType, targetTier)) return -1;
            
            var currentTier = GetComponentTier(componentType);
            double totalCost = 0;
            
            // Fibonacci-like exponential growth
            var componentMultiplier = GetComponentMultiplier(componentType);
            
            for (int tier = currentTier + 1; tier <= targetTier; tier++)
            {
                totalCost += GetTierBaseCost(tier) * componentMultiplier;
            }
            
            return totalCost;
        }

        public bool UpgradeComponent(string componentType, double cost)
        {
            var currentTier = GetComponentTier(componentType);
            var targetTier = currentTier + 1;
            
            if (!CanUpgrade(componentType, targetTier)) return false;
            
            SetComponentTier(componentType, targetTier);
            TotalSpent += cost;
            LastUpgrade = DateTimeOffset.UtcNow;
            return true;
        }

        public HardwareComponent? GetComponentDetails(string componentType)
        {
            var tier = GetComponentTier(componentType);
            
            return componentType.ToUpper() switch
            {
                "CPU" => HardwareComponents.CPUs.FirstOrDefault(c => c.Tier == tier),
                "RAM" => HardwareComponents.RAMs.FirstOrDefault(c => c.Tier == tier),
                "GPU" => HardwareComponents.Gpus.FirstOrDefault(c => c.Tier == tier),
                "PSU" => HardwareComponents.Psu.FirstOrDefault(c => c.Tier == tier),
                _ => null
            };
        }

        public double CalculateTotalHashrate()
        {
            // Same calculation as MiningService but exposed here
            var cpuPower = CpuTier * 0.0001;
            var gpuPower = GpuTier * 0.0003;
            var ramBonus = RamTier * 0.00005;
            var psuEfficiency = PsuTier * 0.00002;
            
            return cpuPower + gpuPower + ramBonus + psuEfficiency;
        }

        public bool HasSufficientPSU()
        {
            var maxComponentTier = Math.Max(Math.Max(CpuTier, GpuTier), Math.Max(RamTier, DiskTier));
            return PsuTier >= maxComponentTier;
        }

        public object GetUpgradeInfo()
        {
            return new {
                CurrentSpecs = new {
                    CPU = GetComponentDetails("CPU")?.Model ?? "Unknown",
                    GPU = GetComponentDetails("GPU")?.Model ?? "Unknown", 
                    RAM = GetComponentDetails("RAM")?.Model ?? "Unknown",
                    PSU = GetComponentDetails("PSU")?.Model ?? "Unknown"
                },
                TotalHashrate = CalculateTotalHashrate(),
                TotalSpent = TotalSpent,
                LastUpgrade = LastUpgrade,
                PSUSufficient = HasSufficientPSU()
            };
        }

        private int GetComponentTier(string componentType)
        {
            return componentType.ToUpper() switch
            {
                "CPU" => CpuTier,
                "RAM" => RamTier,
                "GPU" => GpuTier,
                "DISK" => DiskTier,
                "PSU" => PsuTier,
                _ => 0
            };
        }

        private void SetComponentTier(string componentType, int tier)
        {
            switch (componentType.ToUpper())
            {
                case "CPU": CpuTier = tier; break;
                case "RAM": RamTier = tier; break;
                case "GPU": GpuTier = tier; break;
                case "DISK": DiskTier = tier; break;
                case "PSU": PsuTier = tier; break;
            }
        }

        private double GetTierBaseCost(int tier)
        {
            // Fibonacci-like exponential growth: F(n) = F(n-1) + F(n-2) + base_increment
            // Modified Fibonacci sequence for upgrade costs
            return tier switch
            {
                1 => 0,      // Tier 1 is starting tier (no cost)
                2 => 100,    // First upgrade
                3 => 200,    // 100 + 100
                4 => 400,    // 200 + 200
                5 => 800,    // 400 + 400  
                6 => 1600,   // 800 + 800
                7 => 3200,   // 1600 + 1600
                _ => 0
            };
        }

        private double GetComponentMultiplier(string componentType)
        {
            return componentType.ToUpper() switch
            {
                "CPU" => 1.0,   // Base cost
                "RAM" => 0.8,   // 20% cheaper
                "GPU" => 2.0,   // 2x more expensive (most important for mining)
                "DISK" => 0.6,  // 40% cheaper
                "PSU" => 1.2,   // 20% more expensive
                _ => 1.0
            };
        }
    }