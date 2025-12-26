п»їusing FoodPreOrder.Application.DTOs.Admin;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FoodPreOrder.Api.Services
{
    public class PdfReportService
    {
        public PdfReportService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateFinancialReport(string restaurantName, DateTime from, DateTime to, List<DailyStatsDto> stats)
        {
            return CreateDocument(restaurantName, $"Р¤С–РЅР°РЅСЃРѕРІРёР№ Р—РІС–С‚ ({from:dd.MM} - {to:dd.MM})", container =>
            {
                container.Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(80);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderStyle).Text("Р”Р°С‚Р°");
                        header.Cell().Element(HeaderStyle).Text("Р—Р°РјРѕРІР»РµРЅРЅСЏ");
                        header.Cell().Element(HeaderStyle).Text("РЎРµСЂ. С‡РµРє");
                        header.Cell().Element(HeaderStyle).Text("Р’РёСЂСѓС‡РєР°").AlignRight();
                    });

                    foreach (var item in stats)
                    {
                        table.Cell().Element(CellStyle).Text($"{item.Date:dd.MM.yyyy}");
                        table.Cell().Element(CellStyle).Text($"{item.OrdersCount}");
                        table.Cell().Element(CellStyle).Text($"{item.AverageCheck:F2}");
                        table.Cell().Element(CellStyle).Text($"{item.Revenue:F2}").Bold().AlignRight();
                    }

                    table.Footer(footer =>
                    {
                        footer.Cell().ColumnSpan(4).Element(CellStyle).Text($"Р—РђР“РђР›РћРњ: {stats.Sum(x => x.Revenue):F2} РіСЂРЅ").Bold().AlignRight();
                    });
                });
            });
        }

        public byte[] GenerateDailyLogReport(string restaurantName, DateTime date, List<OrderLogDto> logs)
        {
            return CreateDocument(restaurantName, $"РћРїРµСЂР°С†С–Р№РЅРёР№ Р¶СѓСЂРЅР°Р» Р·Р° {date:dd.MM.yyyy}", container =>
            {
                container.Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(50);
                        columns.ConstantColumn(100);
                        columns.RelativeColumn();
                        columns.ConstantColumn(60);
                        columns.ConstantColumn(70);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderStyle).Text("Р§Р°СЃ");
                        header.Cell().Element(HeaderStyle).Text("РљР»С–С”РЅС‚");
                        header.Cell().Element(HeaderStyle).Text("Р—Р°РјРѕРІР»РµРЅРЅСЏ");
                        header.Cell().Element(HeaderStyle).Text("РЎСѓРјР°");
                        header.Cell().Element(HeaderStyle).Text("РЎС‚Р°С‚СѓСЃ");
                    });

                    foreach (var item in logs)
                    {
                        table.Cell().Element(CellStyle).Text($"{item.Time:HH:mm}");
                        table.Cell().Element(CellStyle).Text(item.CustomerName);
                        table.Cell().Element(CellStyle).Text(item.ItemsSummary).FontSize(9);
                        table.Cell().Element(CellStyle).Text($"{item.TotalAmount:F0}");

                        string statusColor = item.Status == "Completed" ? Colors.Green.Medium :
                                             item.Status == "Cancelled" ? Colors.Red.Medium : Colors.Grey.Darken1;

                        table.Cell().Element(CellStyle).Text(item.Status).FontColor(statusColor).Bold();
                    }
                });
            });
        }

        public byte[] GeneratePeakHoursReport(string restaurantName, List<PeakLoadDto> peaks)
        {
            return CreateDocument(restaurantName, "РђРЅР°Р»С–Р· РЅР°РІР°РЅС‚Р°Р¶РµРЅРЅСЏ (Peak Hours)", container =>
            {
                container.Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(60);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderStyle).Text("Р“РѕРґРёРЅР°");
                        header.Cell().Element(HeaderStyle).Text("РљС–Р»СЊРєС–СЃС‚СЊ Р·Р°РјРѕРІР»РµРЅСЊ");
                        header.Cell().Element(HeaderStyle).Text("Р†РЅС‚РµРЅСЃРёРІРЅС–СЃС‚СЊ");
                    });

                    foreach (var item in peaks)
                    {
                        table.Cell().Element(CellStyle).Text($"{item.Hour}:00 - {item.Hour + 1}:00");
                        table.Cell().Element(CellStyle).Text($"{item.OrdersCount}");
                        table.Cell().Element(CellStyle).Text(item.Intensity);
                    }
                });
            });
        }

        public byte[] GenerateSystemDashboardReport(AdminDashboardDto data)
        {
            return CreateDocument("FoodPreOrder Business Intelligence", $"РђРЅР°Р»С–С‚РёС‡РЅРёР№ Р·СЂС–Р· РЅР° {DateTime.Now:dd.MM.yyyy HH:mm}", container =>
            {
                container.Column(column =>
                {
                    column.Item().PaddingBottom(20).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text($"РљРѕСЂРёСЃС‚СѓРІР°С‡С–РІ: {data.TotalUsers}").FontSize(14);
                            c.Item().Text($"РђРєС‚РёРІРЅРёС… Р·Р°РєР»Р°РґС–РІ: {data.ActiveRestaurants}").FontSize(14);
                        });

                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().Text("Р—Р°РіР°Р»СЊРЅРёР№ РѕР±С–Рі:").FontColor(Colors.Grey.Darken1);
                            c.Item().Text($"{data.TotalSystemRevenue:N0} РіСЂРЅ")
                                .FontSize(24).Bold().FontColor(Colors.Green.Darken2);
                        });
                    });

                    column.Item().LineHorizontal(2).LineColor(Colors.Grey.Lighten2);
                    column.Item().PaddingBottom(20);

                    column.Item().PaddingBottom(10).Text("Р•С„РµРєС‚РёРІРЅС–СЃС‚СЊ СЂРµСЃС‚РѕСЂР°РЅС–РІ").FontSize(16).SemiBold();

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("Р РµСЃС‚РѕСЂР°РЅ");
                            header.Cell().Element(HeaderStyle).Text("Staff").AlignCenter();
                            header.Cell().Element(HeaderStyle).Text("Р§РµРєРё").AlignRight();
                            header.Cell().Element(HeaderStyle).Text("Р’РёСЂСѓС‡РєР°").AlignRight();
                            header.Cell().Element(HeaderStyle).Text("Р§Р°СЃС‚РєР°").AlignRight();
                        });

                        foreach (var rest in data.RestaurantStats)
                        {
                            table.Cell().Element(CellStyle).Text(rest.RestaurantName).Bold();

                            var staffColor = (rest.StaffCount < 2 && rest.Revenue > 0) ? Colors.Red.Medium : Colors.Black;
                            table.Cell().Element(CellStyle).Text($"{rest.StaffCount}").FontColor(staffColor).AlignCenter();

                            table.Cell().Element(CellStyle).Text($"{rest.OrdersCount}").AlignRight();
                            table.Cell().Element(CellStyle).Text($"{rest.Revenue:N0}").AlignRight();

                            var shareColor = rest.RevenueShare > 20 ? Colors.Green.Darken2 : Colors.Grey.Darken2;
                            table.Cell().Element(CellStyle).Text($"{rest.RevenueShare:F1}%").FontColor(shareColor).Bold().AlignRight();
                        }

                        table.Footer(footer =>
                        {
                            footer.Cell().ColumnSpan(5).Element(CellStyle).Text("* Staff С‡РµСЂРІРѕРЅРёРј = РјРѕР¶Р»РёРІРѕ РЅРµ РІРёСЃС‚Р°С‡Р°С” РїРµСЂСЃРѕРЅР°Р»Сѓ").FontSize(9).FontColor(Colors.Grey.Medium).Italic();
                        });
                    });
                });
            });
        }

        public byte[] GenerateTopDishesReport(string restaurantName, List<TopDishDto> dishes)
        {
            return CreateDocument(restaurantName, "РўРѕРї РїСЂРѕРґР°Р¶С–РІ (Top Dishes)", container =>
            {
                container.Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);
                        columns.RelativeColumn();
                        columns.ConstantColumn(60);
                        columns.ConstantColumn(80);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderStyle).Text("#");
                        header.Cell().Element(HeaderStyle).Text("РЎС‚СЂР°РІР°");
                        header.Cell().Element(HeaderStyle).Text("РџСЂРѕРґР°РЅРѕ");
                        header.Cell().Element(HeaderStyle).Text("Р’РёСЂСѓС‡РєР°").AlignRight();
                    });

                    int rank = 1;
                    foreach (var item in dishes)
                    {
                        table.Cell().Element(CellStyle).Text($"{rank++}");
                        table.Cell().Element(CellStyle).Text(item.Name);
                        table.Cell().Element(CellStyle).Text($"{item.SoldCount} С€С‚.");
                        table.Cell().Element(CellStyle).Text($"{item.TotalRevenue:F2}").AlignRight();
                    }
                });
            });
        }

        private byte[] CreateDocument(string restaurantName, string title, Action<IContainer> content)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                    page.Header().Text(text =>
                    {
                        text.Span(title).SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);
                        text.Span($"\n{restaurantName}").FontSize(12).FontColor(Colors.Grey.Darken2);
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Element(content);

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Р—РіРµРЅРµСЂРѕРІР°РЅРѕ FoodPreOrder | РЎС‚РѕСЂС–РЅРєР° ");
                        x.CurrentPageNumber();
                    });
                });
            });
            return document.GeneratePdf();
        }

        static IContainer HeaderStyle(IContainer container)
        {
            return container.DefaultTextStyle(x => x.SemiBold())
                            .BorderBottom(1).BorderColor(Colors.Grey.Darken2).PaddingVertical(5);
        }

        static IContainer CellStyle(IContainer container)
        {
            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
        }
    }
}
