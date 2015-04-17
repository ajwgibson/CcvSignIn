using CcvSignIn.Model;
using LINQtoCSV;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CcvSignIn
{
    public class CsvService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CsvService));

        /// <summary>
        /// Loads children from a CSV data file.
        /// </summary>
        public List<Child> LoadInputData(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                logger.Error("LoadInputData: filename was null or empty");
                return new List<Child>();
            }
            else
            {
                logger.DebugFormat("LoadInputData: loading data from {0}", filename);

                CsvFileDescription cfd = new CsvFileDescription { SeparatorChar = ',', FirstLineHasColumnNames = true };
                CsvContext context = new CsvContext();

                var tmp = context.Read<Child>(filename, cfd).AsQueryable<Child>();

                var children = (
                    from c in tmp
                    orderby c.Last, c.First
                    select c
                ).ToList();

                foreach (var child in children)
                {
                    logger.DebugFormat("{0,3}: {1},{2}", child.Id, child.Last, child.First);
                }

                return children;
            }
        }

        /// <summary>
        /// Saves children who have been signed in to a CSV file.
        /// </summary>
        public void SaveOutputData(string filename, List<Child> children)
        {
            if (string.IsNullOrEmpty(filename))
            {
                logger.Error("SaveOutputData: filename was null or empty");
                return;
            }

            var signedInChildren = children.Where(c => c.SignedInAt != null).ToArray();

            if (signedInChildren == null || signedInChildren.Length == 0)
            {
                logger.Warn("SaveOutputData: no children have been signed in");
            }

            logger.DebugFormat("SaveOutputData: saving data to {0}", filename);

            CsvFileDescription cfd = new CsvFileDescription { SeparatorChar = ',', FirstLineHasColumnNames = true };
            CsvContext context = new CsvContext();
            context.Write<Child>(signedInChildren, filename);
        }
    }
}
