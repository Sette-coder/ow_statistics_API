// Data/StaticDataSeeder.cs
using ow_backendAPI.Constants;
using ow_backendAPI.Models;

namespace ow_backendAPI.Data;

public static class StaticDataSeeder
{
    private static readonly Map[] Maps =
    {
        new() { Name = "King's Row",             Mode = MapMode.Hybrid     },
        new() { Name = "Watchpoint: Gibraltar",  Mode = MapMode.Escort     },
        new() { Name = "Numbani",                Mode = MapMode.Hybrid     },
        new() { Name = "Dorado",                 Mode = MapMode.Escort     },
        new() { Name = "Hollywood",              Mode = MapMode.Hybrid     },
        new() { Name = "Lijiang Tower",          Mode = MapMode.Control    },
        new() { Name = "Ilios",                  Mode = MapMode.Control    },
        new() { Name = "Nepal",                  Mode = MapMode.Control    },
        new() { Name = "Route 66",               Mode = MapMode.Escort     },
        new() { Name = "Eichenwalde",            Mode = MapMode.Hybrid     },
        new() { Name = "Oasis",                  Mode = MapMode.Control    },
        new() { Name = "Junkertown",             Mode = MapMode.Escort     },
        new() { Name = "Blizzard World",         Mode = MapMode.Hybrid     },
        new() { Name = "Rialto",                 Mode = MapMode.Escort     },
        new() { Name = "Busan",                  Mode = MapMode.Control    },
        new() { Name = "Havana",                 Mode = MapMode.Escort     },
        new() { Name = "New Queen Street",       Mode = MapMode.Push       },
        new() { Name = "Circuit Royal",          Mode = MapMode.Escort     },
        new() { Name = "Colosseo",               Mode = MapMode.Push       },
        new() { Name = "Midtown",                Mode = MapMode.Hybrid     },
        new() { Name = "Paraíso",                Mode = MapMode.Hybrid     },
        new() { Name = "Esperança",              Mode = MapMode.Push       },
        new() { Name = "Shambali Monastery",     Mode = MapMode.Escort     },
        new() { Name = "Antarctic Peninsula",    Mode = MapMode.Control    },
        new() { Name = "New Junk City",          Mode = MapMode.Flashpoint },
        new() { Name = "Suravasa",               Mode = MapMode.Flashpoint },
        new() { Name = "Samoa",                  Mode = MapMode.Control    },
        new() { Name = "Runasapi",               Mode = MapMode.Push       },
        new() { Name = "Aatlis",                 Mode = MapMode.Flashpoint }
    };

    private static readonly Hero[] Heroes =
    {
        new() { Name = "Tracer",          Role = HeroRoles.Damage  },
        new() { Name = "Reaper",          Role = HeroRoles.Damage  },
        new() { Name = "Widowmaker",      Role = HeroRoles.Damage  },
        new() { Name = "Pharah",          Role = HeroRoles.Damage  },
        new() { Name = "Reinhardt",       Role = HeroRoles.Tank    },
        new() { Name = "Mercy",           Role = HeroRoles.Support },
        new() { Name = "Torbjörn",        Role = HeroRoles.Damage  },
        new() { Name = "Hanzo",           Role = HeroRoles.Damage  },
        new() { Name = "Winston",         Role = HeroRoles.Tank    },
        new() { Name = "Zenyatta",        Role = HeroRoles.Support },
        new() { Name = "Bastion",         Role = HeroRoles.Damage  },
        new() { Name = "Symmetra",        Role = HeroRoles.Damage  },
        new() { Name = "Zarya",           Role = HeroRoles.Tank    },
        new() { Name = "Cassidy",         Role = HeroRoles.Damage  },
        new() { Name = "Soldier: 76",     Role = HeroRoles.Damage  },
        new() { Name = "Lúcio",           Role = HeroRoles.Support },
        new() { Name = "Roadhog",         Role = HeroRoles.Tank    },
        new() { Name = "Junkrat",         Role = HeroRoles.Damage  },
        new() { Name = "D.Va",            Role = HeroRoles.Tank    },
        new() { Name = "Mei",             Role = HeroRoles.Damage  },
        new() { Name = "Genji",           Role = HeroRoles.Damage  },
        new() { Name = "Ana",             Role = HeroRoles.Support },
        new() { Name = "Sombra",          Role = HeroRoles.Damage  },
        new() { Name = "Orisa",           Role = HeroRoles.Tank    },
        new() { Name = "Doomfist",        Role = HeroRoles.Tank    },
        new() { Name = "Moira",           Role = HeroRoles.Support },
        new() { Name = "Brigitte",        Role = HeroRoles.Support },
        new() { Name = "Wrecking Ball",   Role = HeroRoles.Tank    },
        new() { Name = "Ashe",            Role = HeroRoles.Damage  },
        new() { Name = "Baptiste",        Role = HeroRoles.Support },
        new() { Name = "Sigma",           Role = HeroRoles.Tank    },
        new() { Name = "Echo",            Role = HeroRoles.Damage  },
        new() { Name = "Sojourn",         Role = HeroRoles.Damage  },
        new() { Name = "Junker Queen",    Role = HeroRoles.Tank    },
        new() { Name = "Kiriko",          Role = HeroRoles.Support },
        new() { Name = "Ramattra",        Role = HeroRoles.Tank    },
        new() { Name = "Lifeweaver",      Role = HeroRoles.Support },
        new() { Name = "Illari",          Role = HeroRoles.Support },
        new() { Name = "Mauga",           Role = HeroRoles.Tank    },
        new() { Name = "Venture",         Role = HeroRoles.Damage  },
        new() { Name = "Juno",            Role = HeroRoles.Support },
        new() { Name = "Hazard",          Role = HeroRoles.Tank    },
        new() { Name = "Freja",           Role = HeroRoles.Damage  },
        new() { Name = "Wuyang",          Role = HeroRoles.Support },
        new() { Name = "Vendetta",        Role = HeroRoles.Damage  },
        new() { Name = "Domina",          Role = HeroRoles.Tank    },
        new() { Name = "Emre",            Role = HeroRoles.Damage  },
        new() { Name = "Mizuki",          Role = HeroRoles.Support },
        new() { Name = "Anran",           Role = HeroRoles.Damage  },
        new() { Name = "Jetpack Cat",     Role = HeroRoles.Support }
    };

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await SeedMapsAsync(db);
        await SeedHeroesAsync(db);
    }

    // ─── Maps ──────────────────────────────────────────────
    private static async Task SeedMapsAsync(AppDbContext db)
    {
        var existingNames = db.Map
            .Select(m => m.Name)
            .ToHashSet();

        var toInsert = Maps
            .Where(m => !existingNames.Contains(m.Name))
            .Select(m => new Map
            {
                Name   = m.Name,
                Mode   = m.Mode,
                ModeId = MapMode.GetModeId(m.Mode)
            })
            .ToList();

        if (!toInsert.Any())
        {
            Console.WriteLine("[StaticDataSeeder] ✅ Maps already seeded — skipping.");
            return;
        }

        db.Map.AddRange(toInsert);
        await db.SaveChangesAsync();
        Console.WriteLine($"[StaticDataSeeder] ✅ Seeded {toInsert.Count} maps.");
    }

    // ─── Heroes ────────────────────────────────────────────
    private static async Task SeedHeroesAsync(AppDbContext db)
    {
        var existingNames = db.Hero
            .Select(h => h.Name)
            .ToHashSet();

        var toInsert = Heroes
            .Where(h => !existingNames.Contains(h.Name))
            .ToList();

        if (!toInsert.Any())
        {
            Console.WriteLine("[StaticDataSeeder] ✅ Heroes already seeded — skipping.");
            return;
        }

        db.Hero.AddRange(toInsert);
        await db.SaveChangesAsync();
        Console.WriteLine($"[StaticDataSeeder] ✅ Seeded {toInsert.Count} heroes.");
    }
}