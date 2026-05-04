// Simple PNG to ICO converter script
// Run with: dotnet script ConvertIcon.csx
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

string pngPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_icon.png");
string icoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");

using var bmp = new Bitmap(pngPath);
using var resized = new Bitmap(bmp, new Size(256, 256));
using var ms = new MemoryStream();
using var fs = new FileStream(icoPath, FileMode.Create);

// ICO header
var bw = new BinaryWriter(fs);
bw.Write((short)0);   // reserved
bw.Write((short)1);   // type: icon
bw.Write((short)1);   // count

// Save PNG data to memory
resized.Save(ms, ImageFormat.Png);
byte[] pngData = ms.ToArray();

// ICO directory entry
bw.Write((byte)0);            // width (0 = 256)
bw.Write((byte)0);            // height (0 = 256)
bw.Write((byte)0);            // color palette
bw.Write((byte)0);            // reserved
bw.Write((short)1);           // color planes
bw.Write((short)32);          // bits per pixel
bw.Write(pngData.Length);     // size of image data
bw.Write(22);                 // offset to image data (6 header + 16 entry = 22)

// Write PNG data
bw.Write(pngData);
bw.Flush();

Console.WriteLine($"ICO created: {icoPath}");
