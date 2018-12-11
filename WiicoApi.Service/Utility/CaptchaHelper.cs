using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace WiicoApi.Service.Utility
{
    public class CaptchaHelper
    {
        /// <summary>
        /// 圖片寬度
        /// </summary>
        private int imageWidth = 130;
        /// <summary>
        /// 圖片高度
        /// </summary>
        private int imageHeight = 150;

        /// <summary>
        /// 文字亮度 - 數值越高越亮 越低越暗 0-255
        /// </summary>
        private const int imageTextColorDepth = 80;

        /// <summary>
        /// 干擾圖片亮度 - 數值越高越亮 越低越暗 0-255
        /// </summary>
        private const int imageInterferenceColorDepth = 230;

        /// <summary>
        /// 驗證碼會隨機產生的字元，如果要用英數大小寫，會避開 l1Oo0 之類的。
        /// </summary>
        //private const string _chars = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";
        private const string authChars = "abdefghjknpqrstuwyABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        //private const string _chars = "0123456789";

        /// <summary>
        /// 亂數產生器
        /// </summary>
        private readonly static Random random = new Random();

        /// <summary>
        /// 背景顏色
        /// </summary>
        private readonly static Color imageBackGroundColor = Color.White;

        /// <summary>
        /// 隨機每個驗證碼字元的字體列表
        /// </summary>
        private readonly static List<Font> textFonts = new string[]  {
                    "Arial", "Arial Black", "Calibri", "Cambria", "Verdana",
                    "Trebuchet MS", "Palatino Linotype", "Georgia", "Constantia",
                    "Consolas", "Comic Sans MS", "Century Gothic", "Candara",
                    "Courier New", "Times New Roman"
                }.Select(f => new Font(f, 18, FontStyle.Bold | FontStyle.Italic)).ToList();

        /// <summary>
        /// 加密字串
        /// </summary>
        /// <param name="input">明文</param>
        /// <returns>密文</returns>
        public string ComputeMd5Hash(string input)
        {
            var encoding = new ASCIIEncoding();
            var bytes = encoding.GetBytes(input);
            var md5Hasher = MD5.Create();
            return BitConverter.ToString(md5Hasher.ComputeHash(bytes));
        }


        /// <summary>
        /// 產生驗證碼的圖片
        /// </summary>
        /// <param name="text">驗證碼</param>
        /// <returns>圖片</returns>
        public Image GenerateCaptchaImage(string text)
        {
            var captchaImg = string.Format("{0}\\captcha\\{1}", ConfigurationManager.AppSettings["DrivePath"].ToString(), "captcha_login.png");

            using (var bmpOut = new Bitmap(captchaImg))
            {
                float orientationAngle = random.Next(0, 999); //359

                var g = Graphics.FromImage(bmpOut);
                //建立刷布
                //   var gradientBrush = new LinearGradientBrush(new Rectangle(0, 0, imageWidth, imageHeight), Color.Pink,Color.White, orientationAngle);
                // g.FillRectangle(gradientBrush, 0, 0, imageWidth, imageHeight); //用於畫空白的布

                int tempRndAngle = 0;
                // 用迴圈目的為讓每一個字的顏色跟角度都不一樣
                for (int i = 0; i < text.Length; i++)
                {
                    // 改變角度
                    tempRndAngle = random.Next(-5, 5);
                    g.RotateTransform(tempRndAngle);

                    // 改變顏色
                    g.DrawString(
                        text[i].ToString(),
                        textFonts[random.Next(0, textFonts.Count)],
                        new SolidBrush(GetRandomColor(imageTextColorDepth)),
                        i * imageWidth / (text.Length + 1) * 1.1f,
                        (float)random.NextDouble() * 30.9f
                    );
                    g.RotateTransform(-tempRndAngle);
                }
                Image response;
                var publicImgPath = ConfigurationManager.AppSettings["DrivePath"].ToString();
                using (var ms = new MemoryStream())
                {
                    bmpOut.Save(string.Format("{0}\\captcha\\{1}.gif", publicImgPath, text), ImageFormat.Gif);
                    bmpOut.Save(ms, ImageFormat.Gif);

                    //   bmpBytes = ms.GetBuffer();
                    response = Image.FromStream(ms);
                    bmpOut.Dispose();
                    ms.Dispose();
                }

                return response;
            }
        }

        /// <summary>
        /// 隨機產生驗證碼
        /// </summary>
        /// <param name="textLength">要幾個字元</param>
        /// <returns>驗證碼</returns>
        public string GenerateRandomText(int textLength)
        {
            var result = new string(Enumerable.Repeat(authChars, textLength)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            return result.ToUpper();
        }

        /// <summary>
        /// 隨機劃出干擾線
        /// </summary>
        /// <param name="g">畫布</param>
        /// <param name="lines">干擾線數量</param>
        private void InterferenceLines(ref Graphics g, int lines)
        {
            for (var i = 0; i < lines; i++)
            {
                var pan = new Pen(GetRandomColor(imageInterferenceColorDepth), 1);
                var points = new Point[random.Next(2, 5)];
                for (int pi = 0; pi < points.Length; pi++)
                {
                    points[pi] = new Point(random.Next(0, imageWidth), random.Next(0, imageHeight));
                }
                // 用多個點建立扭曲的弧線
                g.DrawCurve(pan, points);
            }
        }
        /// <summary>
        /// 給干擾圖
        /// </summary>
        /// <param name="g">畫布</param>
        /// <param name="lines">干擾線數量</param>
        private void InterferenceImage(ref Graphics g)
        {

            int m = Math.Max(imageWidth, imageHeight);
            // Draw the text.
            var hatchBrush = new HatchBrush(
              HatchStyle.LargeConfetti,
              Color.White,
              Color.Black);

            for (int i = 0; i < (int)(imageWidth * imageHeight / 30F); i++)
            {
                int x = random.Next(imageWidth);
                int y = random.Next(imageHeight);
                int w = random.Next(m / 50);
                int h = random.Next(m / 50);
                g.FillEllipse(hatchBrush, x, y, w, h);
            }
            hatchBrush.Dispose();
        }


        /// <summary>
        /// 隨機產生顏色
        /// </summary>
        /// <param name="depth">顏色深度</param>
        /// <returns>顏色</returns>
        private Color GetRandomColor(int depth)
        {
            int red = random.Next(depth);
            int green = random.Next(depth);
            int blue = (red + green > 400) ? 0 : 400 - red - green;
            blue = (blue > depth) ? depth : blue;
            return Color.FromArgb(red, green, blue);
        }
    }
}
