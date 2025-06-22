using Alzheimer_Detection.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using Microsoft.Extensions.Logging;

namespace Alzheimer_Detection.Services
{
    // ✅ Interface en dehors de la classe
    public interface IPdfReportService
    {
        byte[] GeneratePdfReport(User user);
    }

    public class PdfReportService : IPdfReportService
    {
        private readonly ILogger<PdfReportService> _logger;

        public PdfReportService(ILogger<PdfReportService> logger)
        {
            _logger = logger;
        }

        public byte[] GeneratePdfReport(User user)
        {
            try
            {
                if (user == null)
                {
                    _logger.LogError("User object is null");
                    throw new ArgumentNullException(nameof(user));
                }

                QuestPDF.Settings.License = LicenseType.Community;

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Header()
                            .AlignCenter()
                            .Text("Rapport d'Analyse Alzheimer")
                            .SemiBold().FontSize(18);

                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Column(column =>
                            {
                                column.Item().Text(text =>
                                {
                                    text.Span("Nom: ").SemiBold();
                                    text.Span(user.LastName ?? "Non spécifié");
                                });

                                column.Item().Text(text =>
                                {
                                    text.Span("Prénom: ").SemiBold();
                                    text.Span(user.FirstName ?? "Non spécifié");
                                });

                                column.Item().Text(text =>
                                {
                                    text.Span("Âge: ").SemiBold();
                                    text.Span(user.Age?.ToString() ?? "N/A");
                                });

                                column.Item().Text(text =>
                                {
                                    text.Span("Date du rapport: ").SemiBold();
                                    text.Span(DateTime.Now.ToString("dd/MM/yyyy"));
                                });

                                var resultat = user.ResultatTest ?? "non-defini";
                                var status = resultat switch
                                {
                                    "alzheimer-maladi" => "Positif pour Alzheimer",
                                    "non-maladia-alzheimer" => "Négatif pour Alzheimer",
                                    _ => "En attente de résultats"
                                };

                                var statusColor = resultat switch
                                {
                                    "alzheimer-maladi" => Colors.Red.Lighten3,
                                    "non-maladia-alzheimer" => Colors.Green.Lighten3,
                                    _ => Colors.Yellow.Lighten3
                                };

                                column.Item()
                                    .PaddingTop(15)
                                    .Background(statusColor)
                                    .Padding(10)
                                    .Text(text =>
                                    {
                                        text.Span("Résultat: ").SemiBold();
                                        text.Span(status);
                                    });

                                if (user.MRI_Image != null && user.MRI_Image.Length > 0)
                                {
                                    column.Item()
                                        .PaddingTop(20)
                                        .Text("Image IRM analysée:")
                                        .SemiBold();
                                    column.Item().PaddingTop(10).Element(container =>
                                    {
                                        container
                                            .Height(200)
                                            .Width(400)
                                            .Image(user.MRI_Image);
                                    });
                                }
                                else
                                {
                                    column.Item()
                                        .PaddingTop(20)
                                        .Text("Aucune image IRM disponible")
                                        .Italic();
                                }

                                column.Item()
                                    .PaddingTop(20)
                                    .Text("Commentaires:")
                                    .SemiBold();

                                var comments = resultat switch
                                {
                                    "alzheimer-maladi" => "Des signes de la maladie d'Alzheimer ont été détectés...",
                                    "non-maladia-alzheimer" => "Aucun signe caractéristique d'Alzheimer n'a été détecté...",
                                    _ => "Analyse en cours..."
                                };

                                column.Item()
                                    .PaddingTop(5)
                                    .Text(comments);
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("Page ");
                                x.CurrentPageNumber();
                                x.Span(" / ");
                                x.TotalPages();
                            });
                    });
                });

                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans GeneratePdfReport");
                throw;
            }
        }
    }
}
