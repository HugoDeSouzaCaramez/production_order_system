using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProductionOrderApi.Models;
using ProductionOrderApi.Enums;

namespace ProductionOrderApi.Data
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public DatabaseSeeder(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task SeedDatabase()
        {
            try
            {
                var hasProducts = await _context.Product.AnyAsync();
                var hasResources = await _context.Resource.AnyAsync();
                var hasOrders = await _context.ProductionOrder.AnyAsync();

                if (!hasProducts || !hasResources || !hasOrders)
                {
                    Console.WriteLine("Populando banco de dados...");

                    var success = await SeedWithEntityFramework();

                    if (!success)
                    {
                        Console.WriteLine("Fallback: Tentando via script SQL...");
                        await ExecuteSqlScript();
                    }

                    Console.WriteLine("Banco de dados populado com sucesso!");
                }
                else
                {
                    Console.WriteLine("Banco já contém dados. Pulando população.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao popular banco: {ex.Message}");
            }
        }

        private async Task<bool> SeedWithEntityFramework()
        {
            try
            {
                Console.WriteLine("🔧 Populando via Entity Framework...");

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    if (!await _context.Product.AnyAsync())
                    {
                        var products = new[]
                        {
                            new Product { Code = "PROD-001", Description = "Placa de Circuito SMT" },
                            new Product { Code = "PROD-002", Description = "Carcaça Injetada" },
                            new Product { Code = "PROD-003", Description = "Módulo Eletrônico Montado" },
                            new Product { Code = "PROD-004", Description = "Kit Embalado" },
                            new Product { Code = "PROD-005", Description = "Fonte de Alimentação" }
                        };
                        await _context.Product.AddRangeAsync(products);
                    }

                    if (!await _context.Resource.AnyAsync())
                    {
                        var resources = new[]
                        {
                            new Resource {
                                Code = "RES-001",
                                Description = "Linha SMT 01",
                                Status = ResourceStatusEnum.Disponivel
                            },
                            new Resource {
                                Code = "RES-002",
                                Description = "Linha Injeção 02",
                                Status = ResourceStatusEnum.EmUso
                            },
                            new Resource {
                                Code = "RES-003",
                                Description = "Montagem Final 01",
                                Status = ResourceStatusEnum.Disponivel
                            },
                            new Resource {
                                Code = "RES-004",
                                Description = "Embalagem 01",
                                Status = ResourceStatusEnum.Parado
                            },
                            new Resource {
                                Code = "RES-005",
                                Description = "Teste Elétrico 01",
                                Status = ResourceStatusEnum.Disponivel
                            }
                        };
                        await _context.Resource.AddRangeAsync(resources);
                    }

                    await _context.SaveChangesAsync();

                    if (!await _context.ProductionOrder.AnyAsync())
                    {
                        var orders = new[]
                        {
                            new ProductionOrder
                            {
                                OrderNumber = "ORD-1001",
                                ProductCode = "PROD-001",
                                QuantityPlanned = 1000,
                                QuantityProduced = 250,
                                Status = ProductionOrderStatusEnum.EmProducao,
                                StartDate = DateTime.Now.AddHours(-5)
                            },
                            new ProductionOrder
                            {
                                OrderNumber = "ORD-1002",
                                ProductCode = "PROD-002",
                                QuantityPlanned = 500,
                                QuantityProduced = 500,
                                Status = ProductionOrderStatusEnum.Finalizada,
                                StartDate = DateTime.Now.AddDays(-2),
                                EndDate = DateTime.Now.AddDays(-1)
                            },
                            new ProductionOrder
                            {
                                OrderNumber = "ORD-1003",
                                ProductCode = "PROD-003",
                                QuantityPlanned = 800,
                                QuantityProduced = 0,
                                Status = ProductionOrderStatusEnum.Planejada,
                                StartDate = DateTime.Now
                            },
                            new ProductionOrder
                            {
                                OrderNumber = "ORD-1004",
                                ProductCode = "PROD-004",
                                QuantityPlanned = 1200,
                                QuantityProduced = 100,
                                Status = ProductionOrderStatusEnum.EmProducao,
                                StartDate = DateTime.Now.AddHours(-2)
                            },
                            new ProductionOrder
                            {
                                OrderNumber = "ORD-1005",
                                ProductCode = "PROD-005",
                                QuantityPlanned = 300,
                                QuantityProduced = 300,
                                Status = ProductionOrderStatusEnum.Finalizada,
                                StartDate = DateTime.Now.AddDays(-1),
                                EndDate = DateTime.Now
                            }
                        };

                        await _context.ProductionOrder.AddRangeAsync(orders);
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Falha no Entity Framework: {ex.Message}");
                return false;
            }
        }

        private async Task ExecuteSqlScript()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "sql", "setup.sql");

                if (!File.Exists(scriptPath))
                {
                    Console.WriteLine("Arquivo SQL não encontrado.");
                    return;
                }

                var script = await File.ReadAllTextAsync(scriptPath);

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var commands = script.Split("GO", StringSplitOptions.RemoveEmptyEntries);

                foreach (var commandText in commands)
                {
                    if (string.IsNullOrWhiteSpace(commandText.Trim()))
                        continue;

                    using var command = new SqlCommand(commandText, connection);
                    await command.ExecuteNonQueryAsync();
                }

                Console.WriteLine("Dados inseridos via script SQL!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Falha no script SQL: {ex.Message}");
            }
        }

        public static async Task RunSeed(IServiceProvider serviceProvider)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();

                Console.WriteLine("Executando seed via Entity Framework...");
                await seeder.SeedDatabase();
                Console.WriteLine("Seed concluído com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no seed: {ex.Message}");
                throw;
            }
        }
    }
}
