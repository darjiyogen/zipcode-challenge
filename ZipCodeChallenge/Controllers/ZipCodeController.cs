using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipCodeChallenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ZipCodeController : ControllerBase
    {
        // Use SortedList to store zip code.
        // Here key and value both will be same
        public static SortedList<int, int> zipCodes = new SortedList<int, int>() { };

        /// <summary>
        /// Insert Operation
        /// </summary>
        /// <remarks>
        ///     Inserts zip code
        /// </remarks>
        /// <param name="zipCode"></param>
        [HttpGet("Insert")]
        public ActionResult<string> Insert(int zipCode)
        {
            // Only add to list if not present
            if (!zipCodes.ContainsKey(zipCode))
            {
                zipCodes.Add(zipCode, zipCode);
            }
            return $"Zip code {zipCode} inserted.";
        }

        /// <summary>
        /// Delete Operation
        /// </summary>
        /// <remarks>
        ///     Delete zip code
        /// </remarks>
        /// <param name="zipCode"></param>
        [HttpGet("Delete")]
        public ActionResult<string> Delete(int zipCode)
        {
           
            zipCodes.Remove(zipCode);
            return $"Zip code {zipCode} deleted.";
        }

        /// <summary>
        /// Check if Zip exists or not
        /// </summary>
        /// <remarks>
        ///     Checking zip code in a list
        /// </remarks>
        /// <param name="zipCode"></param>
        [HttpGet("Has")]
        public ActionResult<bool> Has(int zipCode)
        {
            return zipCodes.ContainsKey(zipCode);
        }

        /// <summary>
        /// Returns list of Zip codes and combine it if it's in a sequence
        /// </summary>
        /// <remarks>
        ///     return formatted zipcodes, e.g 10223-10225, 39211, 30106-30107
        /// </remarks>
        [HttpGet("Display")]
        public ActionResult<string> Display()
        {
            // Prepare Dictionary with sequential elements at same index in a array
            Dictionary<int, int[]> sequenceZips = new Dictionary<int, int[]>();
            StringBuilder zipCodeString = new StringBuilder();
            int index = 0;

            // If only one zip code is present
            if (zipCodes.Count == 1) {
                return zipCodes.ElementAt(0).Value.ToString();
            }

            // Go through all zip codes, strting with 1st index
            for (int i = 1; i < zipCodes.Count; i++)
            {
                // Get current and previous element of list,
                // As we are starting from 1st index, prev element will be always there.
                int currentZip = zipCodes.ElementAt(i).Value;
                int prevZip = zipCodes.ElementAt(i - 1).Value;

                // For first element, add it directly on 0th index
                if (i == 1) {
                    sequenceZips.Add(index, new int[] { prevZip });
                }

                // Check if prev element and current element is in sequence
                if (currentZip - prevZip == 1)
                {
                    // Get array of zipcode already in sequence, then add current element at index
                    int[] existingZips = sequenceZips.ElementAtOrDefault(index).Value != null ? sequenceZips.ElementAtOrDefault(index).Value : new int[] { };
                    sequenceZips[index] = existingZips.Append(currentZip).ToArray();
                }
                // If zip is not in sequence, simply add it to next index in list
                else
                {
                    index++;
                    sequenceZips.Add(index, new int[] { currentZip});
                }
            }

            // Sample for sequenceZips
            /*
            {
                {0, [10223, 10224, 10225]},
                {1, [30106, 30107]},
                {2, [39211]}
            }
            */
            // Go through our list of sequence Zipcodes
            for (int i = 0; i < sequenceZips.Count; i++) {

                // Take first zip
                zipCodeString.Append(sequenceZips[i].First());

                // If sequence has more then 1 zipcodes, append last zip
                if (sequenceZips[i].Length > 1)
                {
                    zipCodeString.Append("-");
                    zipCodeString.Append(sequenceZips[i].Last()); // Get last for specific index
                }

                // Add "," if not last sequence
                if (i != sequenceZips.Count - 1)
                {
                    zipCodeString.Append(",");
                }
            }

            return zipCodeString.ToString();
        }

    }
}
