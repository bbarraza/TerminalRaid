using MongoDB.Bson.Serialization.Attributes;

namespace TerminalRaid.Models
{
    public class HardwareComponent
    {
        [BsonElement("type")]
        public string Type { get; set; }
        [BsonElement("tier")]
        public int Tier { get; set; }
        [BsonElement("Model")]
        public string Model { get; set; }
        [BsonElement("Specifications")]
        public string Specifications { get; set; }
    }

    public static class HardwareComponents
    {
        public static readonly List<HardwareComponent> CPUs = new()
        {
            new HardwareComponent { Type = "CPU", Tier = 1, Model = "Intel Pentium G4560", Specifications = "2 cores, 4 threads, 3.5GHz, 3MB cache, 54W TDP, HD 610 iGPU, PassMark = 3,520."},
            new HardwareComponent { Type = "CPU", Tier = 2, Model = " AMD Ryzen 3 1200", Specifications = "4 cores / 4 threads, 3.1 GHz base / 3.4 GHz boost, 8 MB L3 cache, 65 W TDP, no iGPU, PassMark = 6,292." },
            new HardwareComponent { Type = "CPU", Tier = 3, Model = "Intel i5-8400", Specifications = "6 cores / 6 threads, 2.8 GHz base / 4.0 GHz boost, 9 MB cache, 65 W TDP, no iGPU, PassMark = 11,600." },
            new HardwareComponent { Type = "CPU", Tier = 4, Model = "AMD Ryzen 5 3600", Specifications = "6 cores / 12 threads, 3.6 GHz base / 4.2 GHz boost, 32 MB cache, 65 W TDP, no iGPU, PassMark = 18,500." },
            new HardwareComponent { Type = "CPU", Tier = 5, Model = "Intel i7-10700K", Specifications = "8 cores / 16 threads, 3.8 GHz base / 5.1 GHz boost, 16 MB cache, 125 W TDP, no iGPU, PassMark = 21,000." },
            new HardwareComponent { Type = "CPU", Tier = 6, Model = "AMD Ryzen 9 5900X", Specifications = "12 cores / 24 threads, 3.7 GHz base / 4.8 GHz boost, 64 MB cache, 105 W TDP, no iGPU, PassMark = 38,000." },
            new HardwareComponent { Type = "CPU", Tier = 7, Model = "Intel i9-13900K", Specifications = "24 cores (8P+16E) / 32 threads, 3.0 GHz base / 5.8 GHz boost, 36 MB cache, 125 W TDP, no iGPU, PassMark = 42,000." }
        };

        public static readonly List<HardwareComponent> RAMs = new()
        {
            new HardwareComponent { Type = "RAM", Tier = 1, Model = "4GB DDR3 1333MHz"},
            new HardwareComponent { Type = "RAM", Tier = 2, Model = "8GB DDR3 1600MHz" },
            new HardwareComponent { Type = "RAM", Tier = 3, Model = "8GB DDR4 2400MHz" },
            new HardwareComponent { Type = "RAM", Tier = 4, Model = "16GB DDR4 3200MHz" },
            new HardwareComponent { Type = "RAM", Tier = 5, Model = "32GB DDR4 3600MHz" },
            new HardwareComponent { Type = "RAM", Tier = 6, Model = "32GB DDR5 5200MHz" },
            new HardwareComponent { Type = "RAM", Tier = 7, Model = "64GB DDR5 6400MHz" },
        };

        public static readonly List<HardwareComponent> Gpus = new()
        {
            new HardwareComponent { Type = "GPU", Tier = 1, Model = "AMD RX 550", Specifications = "2GB GDDR5, 1100 MHz GPU clock, 6000 MHz memory clock, 512 cores / 32 TMUs / 16 ROPs, G3DMark = 2,800" },
            new HardwareComponent { Type = "GPU", Tier = 2, Model = "NVIDIA GTX 1050 Ti", Specifications = "4GB GDDR5, 1290 MHz GPU clock, 7000 MHz memory clock, 768 cores / 48 TMUs / 32 ROPs, G3DMark = 6,000" },
            new HardwareComponent { Type = "GPU", Tier = 3, Model = "AMD RX 580", Specifications = "8GB GDDR5, 1257 MHz GPU clock, 8000 MHz memory clock, 2304 cores / 144 TMUs / 32 ROPs, G3DMark = 10,500" },
            new HardwareComponent { Type = "GPU", Tier = 5, Model = "NVIDIA RTX 3070", Specifications = "8GB GDDR6, 1725 MHz GPU clock, 14000 MHz memory clock, 5888 cores / 184 TMUs / 96 ROPs, G3DMark = 23,000" },
            new HardwareComponent { Type = "GPU", Tier = 6, Model = "AMD RX 7900 XTX", Specifications = "24GB GDDR6, 2400 MHz GPU clock, 20000 MHz memory clock, 6144 cores / 384 TMUs / 128 ROPs, G3DMark = 35,000" },
            new HardwareComponent { Type = "GPU", Tier = 7, Model = "NVIDIA RTX 4090", Specifications = "24GB GDDR6X, 2235 MHz GPU clock, 21000 MHz memory clock, 16384 cores / 512 TMUs / 128 ROPs, G3DMark = 53,000" }

        };
        
        public static readonly List<HardwareComponent> Psu = new()
        {
            new HardwareComponent { Type = "PSU", Tier = 1, Model = "400W 80+" },
            new HardwareComponent { Type = "PSU", Tier = 2, Model = "500W 80+ Bronze" },
            new HardwareComponent { Type = "PSU", Tier = 3, Model = "600W 80+ Bronze" },
            new HardwareComponent { Type = "PSU", Tier = 4, Model = "650W 80+ Gold" },
            new HardwareComponent { Type = "PSU", Tier = 5, Model = "750W 80+ Gold" },
            new HardwareComponent { Type = "PSU", Tier = 6, Model = "850W 80+ Platinum" },
            new HardwareComponent { Type = "PSU", Tier = 7, Model = "1000W 80+ Titanium" }

        };
    }
}