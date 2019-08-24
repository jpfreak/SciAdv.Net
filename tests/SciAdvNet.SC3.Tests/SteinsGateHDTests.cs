using SciAdvNet.SC3.Text;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using Xunit;

namespace SciAdvNet.SC3.Tests
{
    public sealed class SteinsGateHDTests
    {
        [Fact]
        public void DecodingStringsWorks()
        {
            var archive = ZipFile.OpenRead("Data/SteinsGateELITE.zip");

            foreach (var entry in archive.Entries)
            {
                using (var fileStream = OpenScript(entry))
                {
                    SC3Module module = SC3Module.Load(fileStream);

                    Debug.WriteLine("#### " + entry.FullName);

                    foreach (var stringHandle in module.StringTable)
                    {
                        Debug.WriteLine("## " + stringHandle.Id + " : " + stringHandle.Resolve());
                    }
                }
            }
        }


        [Fact]
        public void ApplyTextModify()
        {
            var archive = ZipFile.OpenRead("Data/SteinsGateELITE.zip");

            foreach (var entry in archive.Entries)
            {
                using (var fileStream = OpenScript(entry))
                {
                    SC3Module module = SC3Module.Load(fileStream);

                    Debug.WriteLine("#### " + entry.FullName);

                    var modTextFile = new StreamReader("C:/dev/sg_kor_proj/sge-script/" + entry.FullName + ".txt");

                    foreach (var stringHandle in module.StringTable)
                    {
                        var text = modTextFile.ReadLine();

                        Debug.WriteLine(stringHandle.Resolve());
                        Debug.WriteLine(text);

                        module.UpdateString(stringHandle.Id, SC3String.Deserialize(text));
                    }

                    module.ApplyPendingUpdates();

                    using (var fileStream2 = File.Create("C:/Users/mike/Desktop/script_orig/" + entry.FullName))
                    {
                        fileStream.Seek(0, SeekOrigin.Begin);
                        fileStream.CopyTo(fileStream2);
                    }

                    modTextFile.Close();
                }
            }
        }


        [Fact]
        public void SaveScripts()
        {
            var archive = ZipFile.OpenRead("Data/SteinsGateELITE.zip");

            foreach (var entry in archive.Entries)
            {
                using (var fileStream = OpenScript(entry))
                {
                    SC3Module module = SC3Module.Load(fileStream);

                    Debug.WriteLine("#### " + entry.FullName);

                    var modTextFile = new StreamWriter("C:/dev/sg_kor_proj/sge-script/orig/" + entry.FullName + ".txt");

                    foreach (var stringHandle in module.StringTable)
                    {
                        modTextFile.WriteLine(stringHandle.Resolve());
                    }

                    modTextFile.Close();
                }
            }
        }

        private Stream OpenScript(ZipArchiveEntry entry)
        {
            using (var entryStream = entry.Open())
            {
                var seekable = new MemoryStream((int)entryStream.Length);
                entryStream.CopyTo(seekable);
                seekable.Position = 0;
                return seekable;
            }
        }
    }
}
