using MGSimpleForms.Attributes;
using MGSimpleForms.Form;
using MGSimpleForms.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGSimpleFormsExamples.FormExamples
{
    internal class ProgressBarExample : FormViewModel
    {

        [ProgressBar]
        public IProgress<(int, int)> progress { get; set; }

        [Button]
        [Name("Increment")]
        public Command btn => new Command(test);
        [Button]
        [Name("reset")]
        public Command btnreset => new Command(reset);

        private void reset()
        {
            start = 0;
            len = 2;   
            progress?.Report((start, len));
        }

        int start = 0;
        int len = 2;
        private void test()
        {
            start++;
            if (start == len - 1)
            {
                len *= 2;
            }

            progress?.Report((start, len));
        }
    }
}
