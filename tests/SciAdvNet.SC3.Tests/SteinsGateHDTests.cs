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
                    SC3Module module = null;

                    try {
                        module = SC3Module.Load(fileStream);
                    } catch {
                        Debug.WriteLine("#### open file Error : " + entry.FullName);

                        continue;
                    }

                    Debug.WriteLine("#### " + entry.FullName);

                    var modTextFile = new StreamReader("D:/dev/sge-script/" + entry.FullName + ".txt");

                    foreach (var stringHandle in module.StringTable)
                    {
                        var text = modTextFile.ReadLine();

                        Debug.WriteLine(stringHandle.Resolve());
                        Debug.WriteLine(text);

                        module.UpdateString(stringHandle.Id, SC3String.Deserialize(text));
                    }

                    module.ApplyPendingUpdates();

                    using (var fileStream2 = File.Create("C:/Users/jpkim/Desktop/script_orig/" + entry.FullName))
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
                    SC3Module module = null;

                    try {
                        module = SC3Module.Load(fileStream);
                    } catch {
                        Debug.WriteLine("#### open file Error : " + entry.FullName);

                        continue;
                    }

                    var modTextFile = new StreamWriter("C:/Users/jpkim/Desktop/extractTest/" + entry.FullName + ".txt");

                    foreach (var stringHandle in module.StringTable)
                    {
                        try {
                            modTextFile.WriteLine(stringHandle.Resolve());
                        } catch {
                            Debug.WriteLine("#### write error File : " + entry.FullName);
                            break;
                        }
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
