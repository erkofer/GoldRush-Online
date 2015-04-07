using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using JetBrains.Annotations;

namespace Caroline.Extensions
{
    public static class ModelStateEx
    {
        public static IEnumerable<string> GetErrors([NotNull] this IEnumerable<KeyValuePair<string, ModelState>> modelstate)
        {
            if (modelstate == null) throw new ArgumentNullException("modelstate");
            var errors = new List<string>();
            foreach (var field in modelstate)
            {
                errors.AddRange(field.Value.Errors.Select(error => error.ErrorMessage));
            }
            return errors;
        }
    }
}