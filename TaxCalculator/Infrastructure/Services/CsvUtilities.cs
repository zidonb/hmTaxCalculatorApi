namespace TaxCalculator.Infrastructure.Services;

using System;
using System.Collections.Generic;
using System.IO;

public static class CsvUtilities {
    public static double GetFromCsv(string key) {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.GLOBAL_PARAMETERS_CSV_FILE_NAME);

        if (!File.Exists(filePath)) {
            throw new FileNotFoundException("The CSV file was not found.", filePath);
        }

        using (var reader = new StreamReader(filePath)) {
            string? line;
            while ((line = reader.ReadLine()) != null) {
                // Split each line by comma
                var parts = line.Split(',');

                // Check if the line has the expected format
                if (parts.Length < 2) {
                    continue; // Skip malformed lines
                }

                // Trim each part and compare the key
                if (parts[0].Trim().Equals(key, StringComparison.OrdinalIgnoreCase)) {
                    // Parse and return the value
                    if (double.TryParse(parts[1].Trim(), out double value)) {
                        return value;
                    }
                    else {
                        throw new InvalidDataException($"CSV format is incorrect. Unable to parse the value for key '{key}'.");
                    }
                }
            }
        }

        // If the key is not found, throw an exception or return a default value
        throw new KeyNotFoundException($"Key '{key}' not found in the CSV file.");
    }
}

