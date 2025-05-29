using CsvHelper;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace lib
{
    public partial class Form1 : Form
    {
        private string csvPath = "";
        private string logoPath = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void btnPilihCSV_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog { Filter = "CSV Files|*.csv" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                csvPath = ofd.FileName;
                lblCSV.Text = csvPath;
            }
        }

        private void btnPilihLogo_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog { Filter = "Image Files|*.png;*.jpg;*.jpeg" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                logoPath = ofd.FileName;
                lblLogo.Text = logoPath;
            }
        }

        private void btnGeneratePDF_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(csvPath) || string.IsNullOrEmpty(logoPath))
            {
                MessageBox.Show("Pilih file CSV dan logo terlebih dahulu!");
                return;
            }

            using SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PDF Files|*.pdf",
                FileName = "Hasil_Rekap_EDOM.pdf"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    GeneratePDF(csvPath, logoPath, sfd.FileName);
                    MessageBox.Show("PDF berhasil dibuat:\n" + sfd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal generate PDF:\n" + ex.Message);
                }
            }
        }

        private void GeneratePDF(string csvPath, string logoPath, string outputPdf)
        {
            // Configure QuestPDF license
            QuestPDF.Settings.License = LicenseType.Community;

            // Read CSV data
            List<dynamic> records;
            using (var reader = new StreamReader(csvPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = csv.GetRecords<dynamic>().ToList();
            }

            if (!records.Any())
                throw new Exception("CSV kosong.");

            var headers = ((IDictionary<string, object>)records[0]).Keys.ToList();

            string kolDosen = "Nama Dosen";
            string kolMatkul = "Mata Kuliah";
            string? kolSaran = headers.FirstOrDefault(h => h.Contains("Sampaikan saran/masukan"));

            var pertanyaanCols = headers
                .Where(h => h != "Response ID" && h != kolDosen && h != kolMatkul && h != kolSaran)
                .ToList();

            var nilaiMap = new Dictionary<string, double>
            {
                ["Sangat Setuju"] = 4,
                ["Setuju"] = 3,
                ["Cukup Setuju"] = 2,
                ["Tidak Setuju"] = 1
            };

            var groups = records.GroupBy(r =>
            {
                var dict = (IDictionary<string, object>)r;
                string dosen = dict.ContainsKey(kolDosen) && dict[kolDosen] != null ? dict[kolDosen].ToString() ?? "" : "";
                string matkul = dict.ContainsKey(kolMatkul) && dict[kolMatkul] != null ? dict[kolMatkul].ToString() ?? "" : "";
                return (dosen, matkul);
            }).ToList();

            // Create PDF document with QuestPDF
            Document.Create(container =>
            {
                foreach (var group in groups)
                {
                    var (dosen, matkul) = group.Key;

                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontFamily("Helvetica"));

                        // Header with logo and justified text
                        page.Header().Row(row =>
                        {
                            row.ConstantItem(100).Image(logoPath).FitWidth();
                            row.RelativeItem().Column(column =>
                            {
                                // Justify the text while keeping it aligned to the left
                                column.Item().AlignLeft().Text("Evaluasi Dosen oleh Taruna").FontSize(16).Bold();
                                column.Item().AlignLeft().Text("Politeknik Siber dan Sandi Negara | Jurusan Kriptografi").FontSize(12);
                                column.Item().AlignLeft().Text("Semester Gasal T.A. 2024/2025").FontSize(12);
                                column.Item().AlignLeft().Text($"{dosen}\n{matkul}").FontSize(12).Bold();
                            });
                        });

                        // Content with table and suggestions in a single column
                        page.Content().Column(column =>
                        {
                            double totalSkor = 0;
                            int totalJumlah = 0;
                            string currentCategory = "";
                            List<(string aspek, int jumlahSetuju, double skor)> categoryRows = new List<(string, int, double)>();

                            // Process all questions to group by category
                            foreach

 (var aspek in pertanyaanCols)
                            {
                                // Determine the category based on the question prefix
                                string newCategory = "";
                                if (aspek.Contains("Penilaian Pedagogik"))
                                    newCategory = "Kompetensi Pedagogik";
                                else if (aspek.Contains("Penilaian Kompetensi Profesional"))
                                    newCategory = "Kompetensi Profesional Dosen";
                                else if (aspek.Contains("Penilaian kepribadian"))
                                    newCategory = "Kepribadian Dosen";
                                else if (aspek.Contains("Metode pembelajaran"))
                                    newCategory = "Metode Pembelajaran";
                                else if (aspek.Contains("Dosen Menggunakan media pembelajaran") || aspek.Contains("Dosen menggunakan media pembelajaran"))
                                    newCategory = "Media Pembelajaran";
                                else if (aspek.Contains("Kelas"))
                                    newCategory = "Kelas";

                                // Extract the core question from within the square brackets
                                string coreQuestion = aspek;
                                if (aspek.Contains("[") && aspek.Contains("]"))
                                {
                                    int startIndex = aspek.IndexOf("[") + 1;
                                    int endIndex = aspek.LastIndexOf("]");
                                    if (endIndex > startIndex)
                                    {
                                        coreQuestion = aspek.Substring(startIndex, endIndex - startIndex);
                                    }
                                }

                                // Calculate jumlah (count of responses) and skor (total score)
                                int jumlahSetuju = group.Count(r =>
                                {
                                    var dict = (IDictionary<string, object>)r;
                                    return dict.ContainsKey(aspek) && dict[aspek] != null && nilaiMap.ContainsKey(dict[aspek].ToString() ?? "");
                                });

                                double skor = group.Sum(r =>
                                {
                                    var dict = (IDictionary<string, object>)r;
                                    if (dict.ContainsKey(aspek) && dict[aspek] != null && nilaiMap.TryGetValue(dict[aspek].ToString() ?? "", out var val))
                                        return val;
                                    return 0;
                                });

                                // If the category changes and there are rows to render, render the previous category
                                if (newCategory != currentCategory && categoryRows.Any())
                                {
                                    // Render the category table
                                    column.Item().PaddingTop(10).Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(3); // Aspek/Pertanyaan
                                            columns.RelativeColumn(1); // Nilai
                                            columns.RelativeColumn(1); // Nilai Rata-Rata Kelas
                                        });

                                        // Table header for the category
                                        table.Header(header =>
                                        {
                                            header.Cell().Background("#003087").Padding(5).Text("Aspek/Pertanyaan").FontColor("#FFFFFF").FontSize(10);
                                            header.Cell().Background("#003087").Padding(5).Text("Nilai").FontColor("#FFFFFF").FontSize(10);
                                            header.Cell().Background("#003087").Padding(5).Text("Nilai Rata-Rata Kelas").FontColor("#FFFFFF").FontSize(10);
                                        });

                                        // Add the category header row
                                        table.Cell().ColumnSpan(3).Padding(5).Text(currentCategory).FontSize(10).Bold().Italic();

                                        // Add the rows for this category
                                        foreach (var row in categoryRows)
                                        {
                                            double rataRata = row.jumlahSetuju > 0 ? row.skor / row.jumlahSetuju : 0;
                                            table.Cell().Border(0.5f).Padding(5).Text(row.aspek).FontSize(10);
                                            table.Cell().Border(0.5f).Padding(5).Text(row.skor.ToString("0.00")).FontSize(10);
                                            table.Cell().Border(0.5f).Padding(5).Text(rataRata.ToString("0.00")).FontSize(10);
                                        }
                                    });

                                    categoryRows.Clear();
                                }

                                // Update the current category
                                if (newCategory != "")
                                {
                                    currentCategory = newCategory;
                                }

                                // Add the current row to the category's rows
                                categoryRows.Add((coreQuestion, jumlahSetuju, skor));

                                totalSkor += skor;
                                totalJumlah += jumlahSetuju;
                            }

                            // Render the last category
                            if (categoryRows.Any())
                            {
                                column.Item().PaddingTop(10).Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3); // Aspek/Pertanyaan
                                        columns.RelativeColumn(1); // Nilai
                                        columns.RelativeColumn(1); // Nilai Rata-Rata Kelas
                                    });

                                    // Table header for the category
                                    table.Header(header =>
                                    {
                                        header.Cell().Background("#003087").Padding(5).Text("Aspek/Pertanyaan").FontColor("#FFFFFF").FontSize(10);
                                        header.Cell().Background("#003087").Padding(5).Text("Nilai").FontColor("#FFFFFF").FontSize(10);
                                        header.Cell().Background("#003087").Padding(5).Text("Nilai Rata-Rata Kelas").FontColor("#FFFFFF").FontSize(10);
                                    });

                                    // Add the category header row
                                    table.Cell().ColumnSpan(3).Padding(5).Text(currentCategory).FontSize(10).Bold().Italic();

                                    // Add the rows for this category
                                    foreach (var row in categoryRows)
                                    {
                                        double rataRata = row.jumlahSetuju > 0 ? row.skor / row.jumlahSetuju : 0;
                                        table.Cell().Border(0.5f).Padding(5).Text(row.aspek).FontSize(10);
                                        table.Cell().Border(0.5f).Padding(5).Text(row.skor.ToString("0.00")).FontSize(10);
                                        table.Cell().Border(0.5f).Padding(5).Text(rataRata.ToString("0.00")).FontSize(10);
                                    }
                                });
                            }

                            // Total row in a separate table
                            column.Item().PaddingTop(10).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3); // Aspek/Pertanyaan
                                    columns.RelativeColumn(1); // Nilai
                                    columns.RelativeColumn(1); // Nilai Rata-Rata Kelas
                                });

                                double totalRataRata = totalJumlah > 0 ? totalSkor / totalJumlah : 0;
                                table.Cell().Border(0.5f).Padding(5).Text("Total").FontSize(10).Bold();
                                table.Cell().Border(0.5f).Padding(5).Text(totalSkor.ToString("0.00")).FontSize(10).Bold();
                                table.Cell().Border(0.5f).Padding(5).Text(totalRataRata.ToString("0.00")).FontSize(10).Bold();
                            });

                            // Suggestions section
                            if (!string.IsNullOrEmpty(kolSaran))
                            {
                                var saranList = group
                                    .Select(r => (IDictionary<string, object>)r)
                                    .Where(d => d.ContainsKey(kolSaran) && d[kolSaran] != null)
                                    .Select(d => d[kolSaran].ToString())
                                    .Where(s => !string.IsNullOrEmpty(s))
                                    .ToList();

                                if (saranList.Any())
                                {
                                    column.Item().PaddingTop(20).Text("Saran dan Masukan:").FontSize(14).Bold();

                                    // Create a table for suggestions
                                    column.Item().PaddingTop(5).Table(table =>
                                    {
                                        // Define table columns: "No." (narrow) and "Saran/Masukan" (wider)
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.ConstantColumn(30); // Fixed width for "No."
                                            columns.RelativeColumn();   // Remaining space for "Saran/Masukan"
                                        });

                                        // Table header
                                        table.Header(header =>
                                        {
                                            header.Cell().Background("#003087").Padding(5).Text("No.").FontColor("#FFFFFF").FontSize(10);
                                            header.Cell().Background("#003087").Padding(5).Text("Saran/Masukan").FontColor("#FFFFFF").FontSize(10);
                                        });

                                        // Add rows with numbered suggestions
                                        for (int i = 0; i < saranList.Count; i++)
                                        {
                                            table.Cell().Border(0.5f).Padding(5).Text((i + 1).ToString()).FontSize(10);
                                            table.Cell().Border(0.5f).Padding(5).Text(saranList[i]).FontSize(10);
                                        }
                                    });
                                }
                            }
                        });
                    });
                }
            }).GeneratePdf(outputPdf);
        }
    }
}