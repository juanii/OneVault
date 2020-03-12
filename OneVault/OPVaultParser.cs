using System;
using System.Collections.Generic;
using System.Text;

namespace OnePIF
{
    public class OPVaultParser
    {
        public List<Records.BaseRecord> Parse(Stream input)
        {
            List<Records.BaseRecord> records = new List<Records.BaseRecord>();

            using (TextReader textReader = new StreamReader(input))
            {
                StringBuilder stringBuilder = null;
                string line = textReader.ReadLine();

                while (line != null)
                {
                    stringBuilder = new StringBuilder();

                    while (line != null && !line.Equals(recordSeparator))
                    {
                        stringBuilder.AppendLine(line);
                        line = textReader.ReadLine();
                    }

                    string stringRecord = stringBuilder.ToString();

                    if (!string.IsNullOrEmpty(stringRecord))
                    {
                        Records.BaseRecord record = JsonConvert.DeserializeObject<Records.BaseRecord>(stringRecord);

                        // Unknown records are currently not imported
                        if (record != null && !(record is Records.UnknownRecord))
                            records.Add(record as Records.BaseRecord);
                    }

                    line = textReader.ReadLine();
                }
            }

            return records;
        }
    }
}
