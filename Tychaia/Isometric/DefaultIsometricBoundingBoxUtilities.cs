using System;

namespace Tychaia
{
    public class DefaultIsometricBoundingBoxUtilities : IIsometricBoundingBoxUtilities
    {
        public bool Overlaps(params IIsometricBoundingBox[] boundingBoxes)
        {
            if (boundingBoxes.Length <= 1)
                return false;
            
            if (boundingBoxes.Length == 2)
            {
                var a = boundingBoxes[0];
                var b = boundingBoxes[1];
                if (a == b)
                {
                    // The same bounding box can't collide with itself.
                    return false;
                }
                var aX2 = a.X + a.Width;
                var aY2 = a.Y + a.Height;
                var aZ2 = a.Z + a.Depth;
                var bX2 = b.X + b.Width;
                var bY2 = b.Y + b.Height;
                var bZ2 = b.Z + b.Depth;
                if (a.X - Math.Abs(a.XSpeed) < bX2 + Math.Abs(b.XSpeed) && aX2 + Math.Abs(a.XSpeed) > b.X - Math.Abs(b.XSpeed) &&
                    a.Y - Math.Abs(a.YSpeed) < bY2 + Math.Abs(b.YSpeed) && aY2 + Math.Abs(a.YSpeed) > b.Y - Math.Abs(b.YSpeed) &&
                    a.Z - Math.Abs(a.ZSpeed) < bZ2 + Math.Abs(b.ZSpeed) && aZ2 + Math.Abs(a.ZSpeed) > b.Z - Math.Abs(b.ZSpeed))
                    return true;
            }
            
            foreach (var a in boundingBoxes)
            {
                foreach (var b in boundingBoxes)
                {
                    if (a == b)
                        continue;
                    var aX2 = a.X + a.Width;
                    var aY2 = a.Y + a.Height;
                    var aZ2 = a.Y + a.Depth;
                    var bX2 = b.X + b.Width;
                    var bY2 = b.Y + b.Height;
                    var bZ2 = b.Y + b.Depth;
                    if (a.X - Math.Abs(a.XSpeed) < bX2 + Math.Abs(b.XSpeed) && aX2 + Math.Abs(a.XSpeed) > b.X - Math.Abs(b.XSpeed) &&
                        a.Y - Math.Abs(a.YSpeed) < bY2 + Math.Abs(b.YSpeed) && aY2 + Math.Abs(a.YSpeed) > b.Y - Math.Abs(b.YSpeed) &&
                        a.Z - Math.Abs(a.ZSpeed) < bZ2 + Math.Abs(b.ZSpeed) && aZ2 + Math.Abs(a.ZSpeed) > b.Z - Math.Abs(b.ZSpeed))
                        return true;
                }
            }
            return false;
        }
    }
}

