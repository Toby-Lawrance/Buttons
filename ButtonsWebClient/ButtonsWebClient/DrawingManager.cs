using Blazor.Extensions;
using Blazor.Extensions.Canvas;
using Blazor.Extensions.Canvas.Canvas2D;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace ButtonsWebClient
{
    public struct Point
    {
        public double X, Y;
    }

    public struct Colour
    {
        public double R, G, B, A;

        private static byte DoubleToByte(double d) => (byte)(Math.Clamp(d, 0.0, 1.0) * 255);
        private static double ByteToDouble(byte b) => b / 255.0;
        public override string ToString() => $"rgb({DoubleToByte(R)},{DoubleToByte(G)},{DoubleToByte(B)})";

        public static implicit operator Colour(Color c) => new Colour { R = ByteToDouble(c.R), G = ByteToDouble(c.G), B = ByteToDouble(c.B), A = ByteToDouble(c.A) };
    }

    public class ZIndexComparer : IComparer<DrawnObject>
    {
        public int Compare(DrawnObject? x, DrawnObject? y)
        {
            if (x is null && y is null) return 0;

            if (x is null) return -1;

            if (y is null) return 1;

            var zIndexCompare = x.ZIndex.CompareTo(y.ZIndex);
            if (zIndexCompare != 0)
            {
                return zIndexCompare;
            }

            return ReferenceEquals(x, y) ? 0 : -1;
        }
    }

    public abstract class DrawnObject
    {
        public int ZIndex = 1;

        public Point OriginPosition;
        public bool ToDelete { get; set; } = false;
        public abstract Task Update(double timeDelta);
        public abstract Task Render(Canvas2DContext drawingContext, double timeDelta);
    }

    public abstract class InteractableObject : DrawnObject
    {
        protected Point[] BoundingPolygon;

        protected InteractableObject(Point[] boundingPolygon)
        {
            BoundingPolygon = boundingPolygon;
            InputSystem.Instance.LeftClick += ClickEventHandler;
        }

        private void ClickEventHandler(object? sender, InputSystem.ClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (HitTest(e.Position))
                {
                    OnClick();
                }
            }
        }

        public virtual bool HitTest(Point click)
        {
            var normalisedClick = new Point { X = click.X - OriginPosition.X, Y = click.Y - OriginPosition.Y };

            bool inside = false;
            for (int i = 0, j = BoundingPolygon.Length - 1; i < BoundingPolygon.Length; j = i++)
            {
                if (BoundingPolygon[i].Y > normalisedClick.Y != BoundingPolygon[j].Y > normalisedClick.Y &&
                     normalisedClick.X < (BoundingPolygon[j].X - BoundingPolygon[i].X) * (normalisedClick.Y - BoundingPolygon[i].Y) / (BoundingPolygon[j].Y - BoundingPolygon[i].Y) + BoundingPolygon[i].X)
                {
                    inside = !inside;
                }
            }

            return inside;
        }
        public abstract void OnClick();
    }

    public class ClickableRectangle : InteractableObject
    {
        private (double width, double height) size;
        public (double width, double heigh) Size
        {
            get => size;
            set
            {
                size = value;
                BoundingPolygon = GetRectanglePoints(size);
            }
        }

        private readonly ScoreCount scoreCount;
        public Colour colour = Color.White;

        public double Velocity = 100.0;

        public ClickableRectangle(double width, double height, ScoreCount count) : base(GetRectanglePoints((width, height)))
        {
            scoreCount = count;
            Size = (width, height);
        }

        public override void OnClick()
        {
            scoreCount.Increment();
            ToDelete = true;
        }

        public override async Task Update(double timeDelta)
        {
            OriginPosition.Y += Velocity * timeDelta;

            if (OriginPosition.Y >= scoreCount.drawingManager.bounds.height)
            {
                ToDelete = true;
                scoreCount.NewRectangle();
            }
        }

        public override async Task Render(Canvas2DContext drawingContext, double timeDelta)
        {
            await drawingContext.SetFillStyleAsync(colour.ToString());
            await drawingContext.FillRectAsync(OriginPosition.X, OriginPosition.Y, size.width, size.height);
        }

        private static Point[] GetRectanglePoints((double width, double height) size) => new Point[]
        {
            new Point { X=0,Y=0 },
            new Point { X=0,Y=size.height },
            new Point { X=size.width,Y=size.height },
            new Point { X=size.width,Y=0 },
            new Point { X=0,Y=0}
        };
    }

    public class ScoreCount : DrawnObject
    {
        private uint score;

        public readonly DrawingManager drawingManager;
        public ScoreCount(DrawingManager dm)
        {
            drawingManager = dm;
            ZIndex = 9999;
        }

        public void Increment()
        {
            score++;
            NewRectangle();
            NewRectangle();
        }

        public void NewRectangle()
        {
            if (drawingManager.objectsToDraw.OfType<ClickableRectangle>().Count() > 10)
            {
                return;
            }

            var newRectangle = new ClickableRectangle(25, 25, this);
            newRectangle.OriginPosition.X = Random.Shared.NextDouble() * drawingManager.bounds.width;
            newRectangle.Velocity = Random.Shared.NextDouble() * 200.0;
            newRectangle.colour = new Colour { A = 1.0, R = Random.Shared.NextDouble(), B = Random.Shared.NextDouble(), G = Random.Shared.NextDouble() };
            drawingManager.objectsToDraw.Add(newRectangle);
        }

        public override Task Update(double timeDelta)
        {
            return Task.CompletedTask;
        }

        public override async Task Render(Canvas2DContext drawingContext, double t)
        {
            Colour c = Color.Red;
            await drawingContext.SetFillStyleAsync(c.ToString());
            await drawingContext.SetTextAlignAsync(TextAlign.Left);
            await drawingContext.SetDirectionAsync(TextDirection.LTR);
            await drawingContext.FillTextAsync(score.ToString(), OriginPosition.X, OriginPosition.Y);
        }
    }

    public class DrawingManager
    {
        private Canvas2DContext context;
        public (int width, int height) bounds;


        public ICollection<DrawnObject> objectsToDraw = new OrderedBag<DrawnObject>(new ZIndexComparer());
        //public ICollection<DrawnObject> objectsToDraw = new List<DrawnObject>();

        public static async Task<DrawingManager> Create(BECanvasComponent canvas)
        {
            return new DrawingManager { context = await canvas.CreateCanvas2DAsync() };
        }

        private DrawingManager()
        {

        }

        private double? lastTimeStamp;
        public async Task RenderFrame(double timeStamp)
        {
            if (lastTimeStamp is null)
            {
                lastTimeStamp = timeStamp;
                return;
            }
            else
            {
                var delta = (double)((timeStamp - lastTimeStamp) / 1000.0);
                lastTimeStamp = timeStamp;

                await context.ClearRectAsync(0, 0, bounds.width, bounds.height);
                await context.SetFillStyleAsync("black");
                await context.FillRectAsync(0, 0, bounds.width, bounds.height);

                var updateable = objectsToDraw.ToArray();
                foreach (var drawnObject in updateable)
                {
                    await drawnObject.Update(delta);
                    await drawnObject.Render(context, delta);
                }

                var deleteable = objectsToDraw.Where(d => d.ToDelete).ToArray();
                foreach (var drawnObject in deleteable)
                {
                    objectsToDraw.Remove(drawnObject);
                }
            }
        }
    }
}
