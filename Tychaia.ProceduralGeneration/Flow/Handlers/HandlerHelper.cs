// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using System;
using System.Drawing;

namespace Tychaia.ProceduralGeneration.Flow
{
    public static class HandlerHelper
    {
        public static void SendStartMessage(
            string message,
            FlowProcessingRequestType type,
            StorageLayer layer,
            Action<FlowProcessingResponse> put)
        {
            var info = new Bitmap(
                128,
                (message.Split(new[] { "\r\n" }, StringSplitOptions.None).Length) * 16);
            var graphics = Graphics.FromImage(info);
            var font = new Font(SystemFonts.DefaultFont, FontStyle.Bold);
            var brush = new SolidBrush(Color.White);
            graphics.Clear(Color.Black);
            graphics.DrawString(message, font, brush, new PointF(0, 0));

            put(new FlowProcessingResponse
            {
                RequestType = type,
                IsStartNotification = true,
                Results = new object[] { layer, info }
            });
        }
    }
}