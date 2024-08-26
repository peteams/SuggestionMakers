using Microsoft.ML.OnnxRuntimeGenAI;
using System.Diagnostics;

namespace GenChat.TheAI
{
    public class ModelHelper : IDisposable
    {
        private readonly Model _model;
        private readonly Tokenizer _tokenizer;
        private bool _disposedValue;

        private ModelHelper(in Model model, in Tokenizer tokenizer)
        {
            _model = model;
            _tokenizer = tokenizer;
        }

        public static ModelHelper Create()
        {
            var modelFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "phi3");

            Debug.Assert(File.Exists(Path.Combine(modelFolder, "config.json")), 
                $"SLM missing from {Path.GetDirectoryName((new StackTrace(true)).GetFrame(0)?.GetFileName())}\\phi3. Use git to obtain content as per comment Assert in source.");
            // Ensure that the installed git has large file support installed:
            // * https://docs.github.com/en/repositories/working-with-files/managing-large-files/installing-git-large-file-storage
            // Pull a SLM from a repository to your machine, for example:
            // * git clone https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx
            // From the cloned repository, copy all the files in the directory Phi-3-mini-4k-instruct-onnx\cpu_and_mobile\cpu-int4-rtn-block-32-acc-level-4 into the directory phi3 beside this source file.

            var model = new Model(modelFolder);
            var tokenizer = new Tokenizer(model);

            var helper = new ModelHelper(model, tokenizer);
            return helper;
        }

        public Model Model => _model;

        public Tokenizer Tokenizer => _tokenizer;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _tokenizer.Dispose();
                    _model.Dispose();
                }

                _disposedValue = true;
            }
        }

        ~ModelHelper()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
