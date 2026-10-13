// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WDAC_Wizard
{
    /// <summary>
    /// Renders a <see cref="PolicyComparer.ComparisonResult"/> to text-based report formats
    /// (CSV, HTML, Markdown). All output is UTF-8 and self-contained.
    /// </summary>
    internal static class PolicyCompareReport
    {
        public enum ReportFormat
        {
            Csv,
            Html,
            Markdown,
        }

        /// <summary>
        /// Writes <paramref name="result"/> to <paramref name="path"/> in the chosen format.
        /// </summary>
        /// <param name="result">Comparison result to render. Must not be null.</param>
        /// <param name="path">Destination file path.</param>
        /// <param name="format">CSV, HTML, or Markdown.</param>
        /// <param name="differencesOnly">When true, only entries marked IsDifferent are included.</param>
        public static void Write(PolicyComparer.ComparisonResult result,
                                 string path,
                                 ReportFormat format,
                                 bool differencesOnly)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            string content;
            switch (format)
            {
                case ReportFormat.Csv:
                    content = BuildCsv(result, differencesOnly);
                    break;
                case ReportFormat.Html:
                    content = BuildHtml(result, differencesOnly);
                    break;
                case ReportFormat.Markdown:
                    content = BuildMarkdown(result, differencesOnly);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format));
            }

            File.WriteAllText(path, content, new UTF8Encoding(false));
        }

        private static IEnumerable<PolicyComparer.LoadedPolicy> ValidPolicies(PolicyComparer.ComparisonResult r)
        {
            return r.Policies.Where(p => p.Policy != null).ToList();
        }

        private static IEnumerable<IGrouping<string, PolicyComparer.ComparisonEntry>> OrderedGroups(
            PolicyComparer.ComparisonResult result, bool differencesOnly)
        {
            return result.Entries
                         .Where(en => !differencesOnly || en.IsDifferent)
                         .GroupBy(en => en.Section)
                         .OrderBy(g => PolicyComparer.OrderOf(g.Key))
                         .ThenBy(g => g.Key, StringComparer.OrdinalIgnoreCase);
        }

        // ----------------- CSV -----------------

        private static string BuildCsv(PolicyComparer.ComparisonResult result, bool differencesOnly)
        {
            var sb = new StringBuilder();
            var policies = ValidPolicies(result).ToList();

            // Header
            sb.Append("Section,Item,Different");
            foreach (var p in policies)
            {
                sb.Append(',').Append(CsvEscape(p.DisplayName));
            }
            sb.AppendLine();

            foreach (var grp in OrderedGroups(result, differencesOnly))
            {
                foreach (var entry in grp.OrderBy(e => e.DisplayName, StringComparer.OrdinalIgnoreCase))
                {
                    sb.Append(CsvEscape(entry.Section)).Append(',');
                    sb.Append(CsvEscape(entry.DisplayName)).Append(',');
                    sb.Append(entry.IsDifferent ? "Yes" : "No");
                    foreach (var p in policies)
                    {
                        entry.Values.TryGetValue(p.DisplayName, out string v);
                        sb.Append(',').Append(CsvEscape(v ?? "<not present>"));
                    }
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        private static string CsvEscape(string value)
        {
            if (value == null) return string.Empty;
            bool needsQuotes = value.IndexOfAny(new[] { ',', '"', '\r', '\n' }) >= 0;
            if (!needsQuotes) return value;
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        // ----------------- HTML -----------------

        private static string BuildHtml(PolicyComparer.ComparisonResult result, bool differencesOnly)
        {
            var sb = new StringBuilder();
            var policies = ValidPolicies(result).ToList();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\"><head><meta charset=\"utf-8\"/>");
            sb.AppendLine("<title>App Control Policy Comparison</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("  body { font-family: 'Segoe UI', Tahoma, Arial, sans-serif; margin: 24px; color: #222; }");
            sb.AppendLine("  h1 { font-size: 1.4em; margin-bottom: 0.2em; }");
            sb.AppendLine("  h2 { font-size: 1.1em; margin-top: 1.6em; border-bottom: 1px solid #ccc; padding-bottom: 4px; }");
            sb.AppendLine("  table { border-collapse: collapse; width: 100%; margin-bottom: 1.2em; font-size: 0.92em; }");
            sb.AppendLine("  th, td { border: 1px solid #ddd; padding: 6px 8px; vertical-align: top; }");
            sb.AppendLine("  th { background: #f2f2f2; text-align: left; }");
            sb.AppendLine("  tr.diff { background: #fff5dc; }");
            sb.AppendLine("  td.missing { color: #888; font-style: italic; }");
            sb.AppendLine("  .meta { color: #555; font-size: 0.9em; margin-bottom: 1em; }");
            sb.AppendLine("  .summary { margin: 1em 0; }");
            sb.AppendLine("  .summary span { display: inline-block; padding: 4px 10px; margin-right: 6px; background: #e7f0fa; border-radius: 4px; }");
            sb.AppendLine("  .summary span.zero { background: #eee; color: #666; }");
            sb.AppendLine("</style></head><body>");

            sb.Append("<h1>App Control Policy Comparison</h1>");
            sb.Append("<div class=\"meta\">Generated ").Append(HtmlEscape(DateTime.Now.ToString("u"))).Append("</div>");

            // Source policies metadata block
            sb.Append("<h2>Policies</h2>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>#</th><th>Display Name</th><th>Path</th><th>Size</th><th>Modified (UTC)</th><th>SHA-256</th></tr>");
            for (int i = 0; i < policies.Count; i++)
            {
                var p = policies[i];
                sb.Append("<tr><td>").Append(i + 1).Append("</td>")
                  .Append("<td>").Append(HtmlEscape(p.DisplayName)).Append("</td>")
                  .Append("<td>").Append(HtmlEscape(p.SourcePath)).Append("</td>")
                  .Append("<td>").Append(FormatBytes(p.FileSizeBytes)).Append("</td>")
                  .Append("<td>").Append(HtmlEscape(p.LastWriteUtc.ToString("u"))).Append("</td>")
                  .Append("<td><code>").Append(HtmlEscape(p.Sha256 ?? string.Empty)).Append("</code></td>")
                  .AppendLine("</tr>");
            }
            sb.AppendLine("</table>");

            // Summary strip
            var counts = SummaryCounts(result);
            sb.Append("<div class=\"summary\">");
            foreach (var kv in counts.OrderBy(kv => PolicyComparer.OrderOf(kv.Key)).ThenBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase))
            {
                string cls = kv.Value == 0 ? "zero" : string.Empty;
                sb.Append("<span class=\"").Append(cls).Append("\">")
                  .Append(HtmlEscape(kv.Key)).Append(": ").Append(kv.Value)
                  .Append("</span>");
            }
            sb.AppendLine("</div>");

            // Each section
            foreach (var grp in OrderedGroups(result, differencesOnly))
            {
                sb.Append("<h2>").Append(HtmlEscape(grp.Key)).Append("</h2>");
                sb.AppendLine("<table>");
                sb.Append("<tr><th>Item</th>");
                foreach (var p in policies) sb.Append("<th>").Append(HtmlEscape(p.DisplayName)).Append("</th>");
                sb.AppendLine("</tr>");

                foreach (var entry in grp.OrderBy(e => e.DisplayName, StringComparer.OrdinalIgnoreCase))
                {
                    sb.Append(entry.IsDifferent ? "<tr class=\"diff\">" : "<tr>");
                    sb.Append("<td>").Append(HtmlEscape(entry.DisplayName)).Append("</td>");
                    foreach (var p in policies)
                    {
                        entry.Values.TryGetValue(p.DisplayName, out string v);
                        if (v == null)
                        {
                            sb.Append("<td class=\"missing\">&lt;not present&gt;</td>");
                        }
                        else
                        {
                            sb.Append("<td>").Append(HtmlEscape(v)).Append("</td>");
                        }
                    }
                    sb.AppendLine("</tr>");
                }
                sb.AppendLine("</table>");
            }

            sb.AppendLine("</body></html>");
            return sb.ToString();
        }

        private static string HtmlEscape(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value.Replace("&", "&amp;")
                        .Replace("<", "&lt;")
                        .Replace(">", "&gt;")
                        .Replace("\"", "&quot;");
        }

        // ----------------- Markdown -----------------

        private static string BuildMarkdown(PolicyComparer.ComparisonResult result, bool differencesOnly)
        {
            var sb = new StringBuilder();
            var policies = ValidPolicies(result).ToList();

            sb.AppendLine("# App Control Policy Comparison");
            sb.AppendLine();
            sb.Append("_Generated ").Append(DateTime.Now.ToString("u")).AppendLine("_");
            sb.AppendLine();

            sb.AppendLine("## Policies");
            sb.AppendLine();
            sb.AppendLine("| # | Display Name | Path | Size | Modified (UTC) | SHA-256 |");
            sb.AppendLine("|---|---|---|---|---|---|");
            for (int i = 0; i < policies.Count; i++)
            {
                var p = policies[i];
                sb.Append("| ").Append(i + 1)
                  .Append(" | ").Append(MdEscape(p.DisplayName))
                  .Append(" | ").Append(MdEscape(p.SourcePath))
                  .Append(" | ").Append(FormatBytes(p.FileSizeBytes))
                  .Append(" | ").Append(p.LastWriteUtc.ToString("u"))
                  .Append(" | `").Append(p.Sha256 ?? string.Empty).Append("`")
                  .AppendLine(" |");
            }
            sb.AppendLine();

            // Summary
            var counts = SummaryCounts(result);
            sb.Append("**Differences:** ");
            sb.AppendLine(string.Join(" · ",
                counts.OrderBy(kv => PolicyComparer.OrderOf(kv.Key))
                      .ThenBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase)
                      .Select(kv => kv.Key + ": " + kv.Value)));
            sb.AppendLine();

            foreach (var grp in OrderedGroups(result, differencesOnly))
            {
                sb.Append("## ").AppendLine(grp.Key);
                sb.AppendLine();

                sb.Append("| Item");
                foreach (var p in policies) sb.Append(" | ").Append(MdEscape(p.DisplayName));
                sb.AppendLine(" |");

                sb.Append("|---");
                foreach (var _ in policies) sb.Append("|---");
                sb.AppendLine("|");

                foreach (var entry in grp.OrderBy(e => e.DisplayName, StringComparer.OrdinalIgnoreCase))
                {
                    sb.Append("| ").Append(MdEscape(entry.DisplayName));
                    foreach (var p in policies)
                    {
                        entry.Values.TryGetValue(p.DisplayName, out string v);
                        sb.Append(" | ").Append(MdEscape(v ?? "<not present>"));
                    }
                    sb.AppendLine(" |");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string MdEscape(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value.Replace("|", "\\|").Replace("\r", string.Empty).Replace("\n", " ");
        }

        // ----------------- Summary helpers -----------------

        /// <summary>
        /// Returns a dictionary of section -> number of entries flagged as different.
        /// Sections with zero differences are still included.
        /// </summary>
        public static Dictionary<string, int> SummaryCounts(PolicyComparer.ComparisonResult result)
        {
            var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            if (result == null) return dict;

            foreach (var entry in result.Entries)
            {
                if (entry == null || string.IsNullOrEmpty(entry.Section)) continue;
                if (!dict.ContainsKey(entry.Section)) dict[entry.Section] = 0;
                if (entry.IsDifferent) dict[entry.Section]++;
            }
            return dict;
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes <= 0) return "0 B";
            string[] units = { "B", "KB", "MB", "GB" };
            double size = bytes;
            int unit = 0;
            while (size >= 1024 && unit < units.Length - 1)
            {
                size /= 1024;
                unit++;
            }
            return string.Format("{0:0.##} {1}", size, units[unit]);
        }
    }
}
