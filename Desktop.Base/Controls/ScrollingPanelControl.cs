//-----------------------------------------------------------------------------
// ScrollingPanelControl.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.Xna.Framework;


namespace Desktop.Base.Controls
{
    public class ScrollingPanelControl : PanelControl
    {
        private ScrollTracker scrollTracker;
        private Matrix _traslationMatrix;

        public ScrollingPanelControl() : base()
        {
            scrollTracker = new ScrollTracker();
            _traslationMatrix = Matrix.CreateTranslation(0, 0, 0);
        }

        public ScrollingPanelControl(Rectangle viewRect)
            : base()
        {
            scrollTracker = new ScrollTracker(viewRect.Width, viewRect.Height);

            _traslationMatrix = Matrix.CreateTranslation(viewRect.X, viewRect.Y, 0.0f);
        }

        public override void Update(GameTime gametime)
        {
            Vector2 size = ComputeSize();
            scrollTracker.CanvasRect.Width = (int)size.X;
            scrollTracker.CanvasRect.Height = (int)size.Y;
            scrollTracker.Update(gametime);

            base.Update(gametime);
        }

        public override void HandleInput(ScreenSystem.InputHelper input)
        {
            scrollTracker.HandleInput(input, _traslationMatrix);
            //base.HandleInput(input);
            HandleInput(input, _traslationMatrix);
        }

        //public override void HandleInput(ScreenSystem.InputHelper input, Matrix traslationMatrix)
        //{
        //    scrollTracker.HandleInput(input);
        //    base.HandleInput(input, traslationMatrix);
        //}

        public override void Draw(DrawContext context)
        {
            // To render the scrolled panel, we just adjust our offset before rendering our child controls as
            // a normal PanelControl
            context.DrawOffset.Y = -scrollTracker.ViewRect.Y;
            base.Draw(context);
        }
    }
}
