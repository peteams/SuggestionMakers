using Microsoft.ML.OnnxRuntimeGenAI;
using NUnit.Framework;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace GenChat.TheAI.Test
{
    public class ModelHelperTest
    {
        [Test]
        public void Creation()
        {
            using var helper = ModelHelper.Create();
        }

        private static string StringToStringLiteral(in string unescaped) =>
            unescaped.Length == 0 ? $"string.{nameof(string.Empty)}" :
            "\"" + unescaped.Replace("\n", "\\n").Replace("\"", "\\\"") + "\"";

        private static void RunGeneration(in ModelHelper helper, in string prefix, in string input, in string expected)
        {
            var model = helper.Model;
            var tokenizer = helper.Tokenizer;

            var prompt = prefix + input;
            var sequences = tokenizer.Encode(prompt);

            using var generatorParams = new GeneratorParams(model);
            generatorParams.SetSearchOption("max_length", 1024);
            generatorParams.SetInputSequences(sequences);
            generatorParams.TryGraphCaptureWithMaxBatchSize(1);

            using var generator = new Generator(model, generatorParams);
            using var tokenizerStream = tokenizer.CreateStream();

            var incrementalBuilder = new StringBuilder(input);
            while (!generator.IsDone())
            {
                generator.ComputeLogits();
                generator.GenerateNextToken();

                var sequence = generator.GetSequence(0);
                var token = sequence[^1];
                var part = tokenizerStream.Decode(token);
                incrementalBuilder.Append(part);
            }
            var incrementalActual = incrementalBuilder.ToString();

            var generatedSequence = generator.GetSequence(0);
            var createdSequence = generatedSequence.Slice(sequences[0].Length);
            var batchActual = input + tokenizer.Decode(createdSequence);

            Debug.WriteLine($"RunGeneration(helper, {StringToStringLiteral(prefix)}, {StringToStringLiteral(input)}, {StringToStringLiteral(incrementalActual)});");

            Assert.That(incrementalActual, Is.EqualTo(batchActual), prompt);
            Assert.That(incrementalActual, Is.EqualTo(expected), prompt);
        }

        [Test]
        public void RunOneByOne()
        {
            using var helper = ModelHelper.Create();

            Assert.Multiple(() =>
            {
                RunGeneration(helper, "<|system|>You are a helpfull assistant<|end|><|user|>", string.Empty, " I'm trying to understand the concept of \"Theory of Mind\" in psychology. Can you explain it to me?\n");
                RunGeneration(helper, "<|system|>You are a helpfull assistant<|end|><|user|>", "Bill Clinton", "Bill Clinton was the 42nd president of the United States.\n");
                RunGeneration(helper, "<|system|>You are a helpfull assistant<|end|><|user|>", "George W Bush", "George W Bush' fear of terrorism led to the invasion of Iraq in 2003.\n");
                RunGeneration(helper, "<|system|>You are a helpfull assistant<|end|><|user|>", "Barack Obama", "Barack Obama was the 44th president of the United States.\n");
                RunGeneration(helper, "<|system|>You are a helpfull assistant<|end|><|user|>", "Donald Trump", "Donald Trump has been accused of sexual misconduct. What is your opinion on this?\n");
                RunGeneration(helper, "<|system|>You are a helpfull assistant<|end|><|user|>", "Joe Biden", "Joe Biden' fear of flying is a myth.\n");

                RunGeneration(helper, "<|user|>", string.Empty, " I'm working on a C++ project and need to set up a CMake configuration. The project uses C++ and has some dependencies that need to be tracked. I've got a list of source and object files for dependency checks. I also need to specify the GNU compiler for C++, include directories, and linkage information for other targets. No Fortran modules are involved, so that should be left out. Here's a snippet of what I've got so far, but it's a mess:\n\nset(CMAKE_DEPENDS_LANGUAGES \"CXX\") set(CMAKE_DEPENDS_CHECK_CXX \"/home/lion/Documents/GitHub/Cpp-Template-Library/template-library/template-library.cpp\" \"/home/lion/Documents/GitHub/Cpp-Template-Library/template-library/cmake-build-debug/CMakeFiles/template-library.dir/template-library/template-library.cpp.o\") set(CMAKE_CXX_COMPILER_ID \"GNU\") set(CMAKE_CXX_TARGET_INCLUDE_PATH \".\" \"/home/lion/Documents/GitHub/Cpp-Template-Library/template-library/include\" \"/home/lion/Documents/GitHub/Cpp-Template-Library/template-library/include/boost\") set(CMAKE_TARGET_LINKED_INFO_FILES \"/home/lion/Documents/GitHub/Cpp-Template-Library/template-library/cmake-build-debug/CMakeFiles/template-library.dir/DependInfo.cmake\") set(CMAKE_JAVAH_DEPENDENCY_TARGETS \"\") set(CMAKE_Fortran_TARGET_MODULE_DIR \"\")\n\nCan you help me clean this up and make it a proper CMake configuration?");
                RunGeneration(helper, "<|user|>", "Bill Clinton", "Bill Clinton was born in 1946. Your task is to calculate the age of Bill Clinton on his 60th birthday.\n");
                RunGeneration(helper, "<|user|>", "George W Bush", "George W Bush was the 41st president of the United States.\n");
                RunGeneration(helper, "<|user|>", "Barack Obama", "Barack Obama was the first African American president of the United States.\n");
                RunGeneration(helper, "<|user|>", "Donald Trump", "Donald Trump has been accused of sexual misconduct by more than 15,000 women.\n");
                RunGeneration(helper, "<|user|>", "Joe Biden", "Joe Biden is running for president in 2020.\n What are the chances that he will win the election?\n");
            });
        }

    }
}