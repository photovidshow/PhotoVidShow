using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;
using ManagedCore;
using System.Globalization;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{ 
	/// <summary>
	/// Contains a table of all the available transition effects
	/// </summary>
	public class CTransitionEffectTable
	{
        private static CTransitionEffectTable mInstance = null;

        private CTransitionEffect[] mEffectsTable = null;

        //*************************************************************************
        public static CTransitionEffectTable GetInstance()
        {
            if (mInstance==null)
            {
                mInstance = new CTransitionEffectTable();
            }
            return mInstance;
        }

        //*************************************************************************
        private CTransitionEffectTable()
        {
        }

        
        //*************************************************************************
        public CTransitionEffect[] GetEffects()
        {
            if (mEffectsTable == null)
            {
                BuildTable();
            }


            return mEffectsTable;
        }

        //*************************************************************************
        private void BuildTable()
        {
            int numEffects = 6 * 7 * 5;

            mEffectsTable = new CTransitionEffect[numEffects];

            // Original effects from version v1 ... v3.1.4
            mEffectsTable[1] = new SimpleAlphaBlendTransitionEffect(1.5f);
            mEffectsTable[2] = new FadeToBWTransitionEffect(1.5f);
            mEffectsTable[3] = new HBlurWhiteout(1.5f);
            mEffectsTable[4] = new BlurTransitionEffect(1.5f);
            mEffectsTable[5] = new AlphaSwipTransitionEffect(1.5f, AlphaSwipTransitionEffect.SwipeDirection.RIGHT, 16);
            mEffectsTable[6] = new AlphaSwipTransitionEffect(1.5f, AlphaSwipTransitionEffect.SwipeDirection.LEFT, 16);
            mEffectsTable[7] = new AlphaSwipTransitionEffect(1.5f, AlphaSwipTransitionEffect.SwipeDirection.RIGHT, 32);
            mEffectsTable[8] = new AlphaSwipTransitionEffect(1.5f, AlphaSwipTransitionEffect.SwipeDirection.LEFT, 32);
            mEffectsTable[9] = new MiddleSaturateToColorTransitionEffect(Color.White, 1.5f);
            mEffectsTable[10] = new MiddleSaturateToColorTransitionEffect(Color.Black, 1.5f);
            mEffectsTable[11] = new FadeUpTransitionEffect(1.5f, 64);
            mEffectsTable[12] = new FadeDownTransitionEffect(1.5f, 64);
            mEffectsTable[13] = new FadeUpTransitionEffect(1.5f, 32);
            mEffectsTable[14] = new FadeDownTransitionEffect(1.5f, 32);
            mEffectsTable[15] = new RandomFizzTransition(1.5f, 16);
            mEffectsTable[16] = new CircleInTransitionEffect(1.5f, 32, 1, 1);
            mEffectsTable[17] = new CircleOutTransitionEffect(1.5f, 32, 1, 1);
            mEffectsTable[18] = new CircleInTransitionEffect(1.5f, 128, 1, 1);
            mEffectsTable[19] = new CircleOutTransitionEffect(1.5f, 128, 1, 1);

            // effects from V4.1.5 onwards
            int index = 20;

            mEffectsTable[index++] = new CloudTransitionEffect(1.5f, 128);
            mEffectsTable[index++] = new CloudTransitionEffect(1.5f, 16);
            mEffectsTable[index++] = new MoveToNextSlideTransitionEffect(0.7f, MoveToNextSlideTransitionEffect.SwipeDirection.UP, 0);
            mEffectsTable[index++] = new MoveToNextSlideTransitionEffect(0.7f, MoveToNextSlideTransitionEffect.SwipeDirection.DOWN, 0);
            mEffectsTable[index++] = new MoveToNextSlideTransitionEffect(0.7f, MoveToNextSlideTransitionEffect.SwipeDirection.LEFT, 0);
            mEffectsTable[index++] = new MoveToNextSlideTransitionEffect(0.7f, MoveToNextSlideTransitionEffect.SwipeDirection.RIGHT, 0);

            mEffectsTable[index++] = new WibbleTransitionEffect(1.5f, 40, 0.06f, 40, 0.06f);
            mEffectsTable[index++] = new WibbleTransitionEffect(1.5f, 0, 0, 10, 0.5f);
            mEffectsTable[index++] = new WibbleTransitionEffect(1.5f, 10, 0.5f, 0, 0);
            mEffectsTable[index++] = new WibbleTransitionEffect(1.5f, 10, 0.4f, 10, 0.4f);
            mEffectsTable[index++] = new WibbleTransitionEffect(1.5f, 120, 0.04f, 120, 0.02f);

            mEffectsTable[index++] = new Shader075ThenAlphaBlendTransitionEffect(1.5f, "binaryColor");
            mEffectsTable[index++] = new ShaderPlusAlphaBlendTransitionEffect(1.5f, "zoom");
            mEffectsTable[index++] = new ShaderPlusAlphaBlendTransitionEffect(1.5f, "spiral");
            mEffectsTable[index++] = new ShaderMixTransitionEffect(1.5f, "spiral");
            mEffectsTable[index++] = new ShaderPlusAlphaBlendTransitionEffect(1.5f, "magnify");
            mEffectsTable[index++] = new ShaderMixTransitionEffect(1.5f, "magnify");
            mEffectsTable[index++] = new Shader075ThenAlphaBlendTransitionEffect(1.5f, "emboss");
            mEffectsTable[index++] = new ShaderMixTransitionEffect(1.5f, "saturate");
            mEffectsTable[index++] = new ShaderMixTransitionEffect(1.5f, "crt");
            mEffectsTable[index++] = new ShaderPlusAlphaBlendTransitionEffect(1.5f, "ripple");
            mEffectsTable[index++] = new ShaderMixTransitionEffect(1.5f, "ripple");
            mEffectsTable[index++] = new ShaderMixTransitionEffect(1.5f, "wibble2");


            mEffectsTable[index++] = new RadialTransitionEffect(0.7f, 15, true, false, false, false, 0);
            mEffectsTable[index++] = new RadialTransitionEffect(0.7f, 15, false, false, false, false, 0);
            mEffectsTable[index++] = new RadialTransitionEffect(0.7f, 15, true, true, false, false, 0);
            mEffectsTable[index++] = new RadialTransitionEffect(0.7f, 15, false, true, false, false, 0);
            mEffectsTable[index++] = new RadialTransitionEffect(0.7f, 15, true, false, true, false, 0);
            mEffectsTable[index++] = new RadialTransitionEffect(0.7f, 15, true, true, true, false, 0);
            mEffectsTable[index++] = new RadialTransitionEffect(0.7f, 15, true, true, true, true, 0);
            mEffectsTable[index++] = new RadialTransitionEffect(0.7f, 15, true, true, true, true, 1);
            mEffectsTable[index++] = new RadialTransitionEffect(0.7f, 15, true, true, true, true, 2);
            mEffectsTable[index++] = new RadialTransitionEffect(0.7f, 15, true, true, true, true, 3);
            mEffectsTable[index++] = new RadialTransitionEffect(0.7f, 15, true, true, true, true, 4);


            mEffectsTable[index++] = new MultiSquaresInOutTransitionEffect(true, 8, 1, 1, 1.5f, 15);
            mEffectsTable[index++] = new MultiSquaresInOutTransitionEffect(false, 8, 1, 1, 1.5f, 15);
            mEffectsTable[index++] = new MultiCirclesInOutTransitionEffect(true, 8, 1, 1, 1.5f, 15);
            mEffectsTable[index++] = new MultiCirclesInOutTransitionEffect(false, 8, 1, 1, 1.5f, 15);
            mEffectsTable[index++] = new MultiDiagonalSquaresInOutTransitionEffect(true, 8, 1, 1, 1.5f, 15);
            mEffectsTable[index++] = new MultiDiagonalSquaresInOutTransitionEffect(false, 8, 1, 1, 1.5f, 15);
            mEffectsTable[index++] = new MultiSquaresAlternateInOutTransitionEffect(true, 8, 1, 1, 1.5f, 15);
            mEffectsTable[index++] = new MultiSquaresAlternateInOutTransitionEffect(false, 8, 1, 1, 1.5f, 15);
            mEffectsTable[index++] = new MultiCrossInOutTransitionEffect(true, 8, 1, 1, 1.5f, 15);
            mEffectsTable[index++] = new MultiCrossInOutTransitionEffect(false, 8, 1, 1, 1.5f, 15);


            mEffectsTable[index++] = new CircleInTransitionEffect(1.5f, 32, 1, 2);
            mEffectsTable[index++] = new CircleInTransitionEffect(1.5f, 32, 2, 1);
            mEffectsTable[index++] = new CircleOutTransitionEffect(1.5f, 32, 1, 2);
            mEffectsTable[index++] = new CircleOutTransitionEffect(1.5f, 32, 2, 1);
            mEffectsTable[index++] = new MiddleSaturateToColorTransitionEffect(Color.White, 1.5f);
            mEffectsTable[index++] = new MiddleSaturateToColorTransitionEffect(Color.Black, 1.5f);
            mEffectsTable[index++] = new CameraSnapshotTransitionEffect(0.3f);
            mEffectsTable[index++] = new DiagonalSquaresSaturateTransitionEffect(Color.White, 5, 10, 0.5f, true, true, 1.5f);
            mEffectsTable[index++] = new DiagonalSquaresSaturateTransitionEffect(Color.White, 5, 10, 0.5f, true, false, 1.5f);
            mEffectsTable[index++] = new DiagonalSquaresSaturateTransitionEffect(Color.White, 5, 10, 0.5f, true, true, 1.5f);
            mEffectsTable[index++] = new DiagonalSquaresSaturateTransitionEffect(Color.White, 5, 10, 0.5f, false, false, 1.5f);
            mEffectsTable[index++] = new DiagonalSquaresSaturateTransitionEffect(Color.Black, 5, 10, 0.5f, true, true, 1.5f);
            mEffectsTable[index++] = new DiagonalSquaresSaturateTransitionEffect(Color.Black, 5, 10, 0.5f, true, false, 1.5f);
            mEffectsTable[index++] = new DiagonalSquaresSaturateTransitionEffect(Color.Black, 5, 10, 0.5f, true, true, 1.5f);
            mEffectsTable[index++] = new DiagonalSquaresSaturateTransitionEffect(Color.Black, 5, 10, 0.5f, false, false, 1.5f);

            mEffectsTable[index++] = new DiagonalSquaresSaturateTransitionEffect(Color.White, 40, 45, 0.8f, true, true, 1.5f);
            mEffectsTable[index++] = new DiagonalSquaresSaturateTransitionEffect(Color.White, 40, 45, 0.8f, false, false, 1.5f);
            mEffectsTable[index++] = new DiagonalSquaresSaturateTransitionEffect(Color.Black, 40, 45, 0.8f, true, true, 1.5f);
            mEffectsTable[index++] = new DiagonalSquaresSaturateTransitionEffect(Color.Black, 40, 45, 0.8f, false, false, 1.5f);

            mEffectsTable[index++] = new MultipleLinesUpDownTransitionEffect(5, true, 1.5f, 10);
            mEffectsTable[index++] = new MultipleLinesUpDownTransitionEffect(5, false, 1.5f, 10);
            mEffectsTable[index++] = new MultipleLinesUpDownTransitionEffect(10, true, 1.5f, 10);
            mEffectsTable[index++] = new MultipleLinesUpDownTransitionEffect(10, false, 1.5f, 10);
            mEffectsTable[index++] = new MultipleLinesUpDownTransitionEffect(20, true, 1.5f, 10);
            mEffectsTable[index++] = new MultipleLinesUpDownTransitionEffect(20, false, 1.5f, 10);

            mEffectsTable[index++] = new MultipleLinesLeftRightTransitionEffect(5, true, 1.5f, 10);
            mEffectsTable[index++] = new MultipleLinesLeftRightTransitionEffect(5, false, 1.5f, 10);
            mEffectsTable[index++] = new MultipleLinesLeftRightTransitionEffect(10, true, 1.5f, 10);
            mEffectsTable[index++] = new MultipleLinesLeftRightTransitionEffect(10, false, 1.5f, 10);
            mEffectsTable[index++] = new MultipleLinesLeftRightTransitionEffect(20, true, 1.5f, 10);
            mEffectsTable[index++] = new MultipleLinesLeftRightTransitionEffect(20, false, 1.5f, 10);

            mEffectsTable[index++] = new MultipleLinesDiagonalTransitionEffect(5, true, 1.5f, 10);
            mEffectsTable[index++] = new MultipleLinesDiagonalTransitionEffect(5, false, 1.5f, 10);
            mEffectsTable[index++] = new MultipleLinesDiagonalTransitionEffect(20, true, 1.5f, 10);
            mEffectsTable[index++] = new MultipleLinesDiagonalTransitionEffect(20, false, 1.5f, 10);

            mEffectsTable[index++] = new MultiCirclesInOutTransitionEffect(true, 5, 1.0f, 1.0f, 1.5f, 10);
            mEffectsTable[index++] = new MultiCirclesInOutTransitionEffect(false, 5, 1.0f, 1.0f, 1.5f, 10);
            mEffectsTable[index++] = new MultiCirclesInOutTransitionEffect(true, 10, 1.0f, 1.0f, 1.5f, 10);
            mEffectsTable[index++] = new MultiCirclesInOutTransitionEffect(false, 10, 1.0f, 1.0f, 1.5f, 10);
            mEffectsTable[index++] = new MultiCirclesInOutTransitionEffect(true, 20, 1.0f, 1.0f, 1.5f, 10);
            mEffectsTable[index++] = new MultiCirclesInOutTransitionEffect(false, 20, 1.0f, 1.0f, 1.5f, 10);

            mEffectsTable[index++] = new MultiCirclesInOutTransitionEffect(true, 10, 2.0f, 1.0f, 1.5f, 10);
            mEffectsTable[index++] = new MultiCirclesInOutTransitionEffect(false, 10, 2.0f, 1.0f, 1.5f, 10);
            mEffectsTable[index++] = new MultiCirclesInOutTransitionEffect(true, 20, 2.0f, 1.0f, 1.5f, 10);
            mEffectsTable[index++] = new MultiCirclesInOutTransitionEffect(false, 20, 2.0f, 1.0f, 1.5f, 10);

            mEffectsTable[index++] = new MultiCirclesInOutTransitionEffect(true, 10, 1.0f, 2.0f, 1.5f, 10);
            mEffectsTable[index++] = new MultiCirclesInOutTransitionEffect(false, 10, 1.0f, 2.0f, 1.5f, 10);
            mEffectsTable[index++] = new MultiCirclesInOutTransitionEffect(true, 20, 1.0f, 2.0f, 1.5f, 10);
            mEffectsTable[index++] = new MultiCirclesInOutTransitionEffect(false, 20, 1.0f, 2.0f, 1.5f, 10);

            mEffectsTable[index++] = new MiddleSplitUpDownTransition(false, 1.5f, 10);
            mEffectsTable[index++] = new MiddleSplitUpDownTransition(true, 1.5f, 20);

            mEffectsTable[index++] = new MiddleSplitLeftRightTransition(false, 1.5f, 10);
            mEffectsTable[index++] = new MiddleSplitLeftRightTransition(true, 1.5f, 20);

            mEffectsTable[index++] = new MiddleSplitDiagonalTransitionEffect(false, false, 1.5f, 10);
            mEffectsTable[index++] = new MiddleSplitDiagonalTransitionEffect(true, false, 1.5f, 20);
            mEffectsTable[index++] = new MiddleSplitDiagonalTransitionEffect(false, true, 1.5f, 10);
            mEffectsTable[index++] = new MiddleSplitDiagonalTransitionEffect(true, true, 1.5f, 20);

            mEffectsTable[index++] = new MiddleColorTransitionEffect(Color.White, 1.5f);
            mEffectsTable[index++] = new MiddleColorTransitionEffect(Color.Black, 1.5f);
            mEffectsTable[index++] = new MiddleColorTransitionEffect(Color.Thistle, 1.5f);
            mEffectsTable[index++] = new MiddleColorTransitionEffect(Color.Cornsilk, 1.5f);


            // vertical stretch out transition effect
            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CVerticalStretch vs = new CVerticalStretch(1, 20);
                vs.Equation = new CNonLinear();
                ade.Stretch = vs;
                MovementTransitionEffect mte = new MovementTransitionEffect(0.3f, ade, null, false);
                mte.DoAlphaBlend = true;
                mEffectsTable[index++] = mte;

            }

            // vertical stretch in transition effect
            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CVerticalStretch vs = new CVerticalStretch(20, 1);
                vs.Equation = new CNonLinear();
                ade.Stretch = vs;
                MovementTransitionEffect mte = new MovementTransitionEffect(0.3f, null, ade, false);
                mte.DoAlphaBlend = true;
                mEffectsTable[index++] = mte;

            }


            // horizontal stretch out transition effect
            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CHorizontalStretch vs = new CHorizontalStretch(1, 20);
                vs.Equation = new CNonLinear();
                ade.Stretch = vs;
                MovementTransitionEffect mte = new MovementTransitionEffect(0.3f, ade, null, false);
                mte.DoAlphaBlend = true;
                mEffectsTable[index++] = mte;

            }

            // horizontal stretch in transition effect
            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CHorizontalStretch vs = new CHorizontalStretch(20, 1);
                vs.Equation = new CNonLinear();
                ade.Stretch = vs;
                MovementTransitionEffect mte = new MovementTransitionEffect(0.3f, null, ade, false);
                mte.DoAlphaBlend = true;
                mEffectsTable[index++] = mte;

            }


            // zoom out transition effect
            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CZoom zoom = new CZoom(1, 20);
                zoom.Equation = new CNonLinear();
                ade.Zoom = zoom;
                MovementTransitionEffect mte = new MovementTransitionEffect(0.3f, ade, null, false);
                mte.DoAlphaBlend = true;
                mEffectsTable[index++] = mte;

            }

            // zoom in transition effect
            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CZoom zoom = new CZoom(20, 1);
                zoom.Equation = new CNonLinear();
                ade.Zoom = zoom;
                MovementTransitionEffect mte = new MovementTransitionEffect(0.3f, null, ade, false);
                mte.DoAlphaBlend = true;
                mEffectsTable[index++] = mte;

            }



            // non linear
            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 0), new PointF(-1, 0));
                slm.Equation = new CLinear();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.3f, ade, null, true);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 0), new PointF(1, 0));
                slm.Equation = new CNonLinear();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, ade, null, true);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 0), new PointF(0, -1));
                slm.Equation = new CNonLinear();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, ade, null, true);

            }
            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 0), new PointF(0, 1));
                slm.Equation = new CNonLinear();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, ade, null, true);
            }


            // non linear 2
            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(-1, 0), new PointF(0, 0));
                slm.Equation = new CNonLinear();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(1, 0), new PointF(0, 0));
                slm.Equation = new CNonLinear();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, -1), new PointF(0, 0));
                slm.Equation = new CNonLinear();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);

            }
            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 1), new PointF(0, 0));
                slm.Equation = new CNonLinear();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);
            }

            // non linear 2 spring
            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(-1, 0), new PointF(0, 0));
                slm.Equation = new CSpring();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(1, 0), new PointF(0, 0));
                slm.Equation = new CSpring();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, -1), new PointF(0, 0));
                slm.Equation = new CSpring();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);

            }
            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 1), new PointF(0, 0));
                slm.Equation = new CSpring();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);
            }



            // diagonal
            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 0), new PointF(-1, -1));
                slm.Equation = new CNonLinear();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, ade, null, true);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 0), new PointF(1, 1));
                slm.Equation = new CNonLinear();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, ade, null, true);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 0), new PointF(-1, -1));
                slm.Equation = new CNonLinear();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, ade, null, true);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 0), new PointF(1, 1));
                slm.Equation = new CNonLinear();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, ade, null, true);
            }

            // diagonal 2

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(-1, -1), new PointF(0, 0));
                slm.Equation = new CNonLinear();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(1, 1), new PointF(0, 0));
                slm.Equation = new CNonLinear();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(-1, -1), new PointF(0, 0));
                slm.Equation = new CNonLinear();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(1, 1), new PointF(0, 0));
                slm.Equation = new CNonLinear();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);
            }

            // diagonal 2 spring

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(-1, -1), new PointF(0, 0));
                slm.Equation = new CSpring();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(1, 1), new PointF(0, 0));
                slm.Equation = new CSpring();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(-1, -1), new PointF(0, 0));
                slm.Equation = new CSpring();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(1, 1), new PointF(0, 0));
                slm.Equation = new CSpring();
                ade.Movement = slm;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);
            }


            // stretch

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CHorizontalStretch hs = new CHorizontalStretch(1, 0);
                hs.Equation = new CNonLinear();
                ade.Stretch = hs;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, ade, null, true);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CVerticalStretch vs = new CVerticalStretch(1, 0);
                vs.Equation = new CNonLinear();
                ade.Stretch = vs;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, ade, null, true);
            }

            // zoom

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CZoom zoom = new CZoom(1, 0.0001f);
                zoom.OneOverZ = false;
                ade.Zoom = zoom;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, ade, null, true);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CZoom zoom = new CZoom(0.00001f, 1);
                zoom.OneOverZ = false;
                ade.Zoom = zoom;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CZoom zoom = new CZoom(0.00001f, 1);
                zoom.OneOverZ = false;
                zoom.Equation = new CSpring();
                ade.Zoom = zoom;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);
            }

            // zoom rotate

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CZoom zoom = new CZoom(1, 0.0001f);
                zoom.OneOverZ = false;
                CRotation rot = new CRotation(0, 720);
                rot.Equation = new CNonLinear();
                ade.Rotation = rot;
                ade.Zoom = zoom;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, ade, null, true);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CZoom zoom = new CZoom(0.00001f, 1);
                zoom.OneOverZ = false;
                CRotation rot = new CRotation(720, 0);
                rot.Equation = new CNonLinear();
                ade.Rotation = rot;
                ade.Zoom = zoom;
                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);
            }



            // rotation
            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();

                ade.Rotation = new CRotation(0, 90);
                ade.Rotation.XOffset = -0.5f;
                ade.Rotation.YOffset = -0.5f;
                ade.Rotation.Equation = new CNonLinear();

                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, ade, null, true);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();

                ade.Rotation = new CRotation(0, -90);
                ade.Rotation.XOffset = 0.5f;
                ade.Rotation.YOffset = -0.5f;
                ade.Rotation.Equation = new CNonLinear();

                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, ade, null, true);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();

                ade.Rotation = new CRotation(90, 0);
                ade.Rotation.XOffset = -0.5f;
                ade.Rotation.YOffset = -0.5f;
                ade.Rotation.Equation = new CNonLinear();

                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();

                ade.Rotation = new CRotation(-90, 0);
                ade.Rotation.XOffset = 0.5f;
                ade.Rotation.YOffset = -0.5f;
                ade.Rotation.Equation = new CNonLinear();

                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);
            }


            // rotation spring

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();

                ade.Rotation = new CRotation(90, 0);
                ade.Rotation.XOffset = -0.5f;
                ade.Rotation.YOffset = -0.5f;
                ade.Rotation.Equation = new CSpring();

                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();

                ade.Rotation = new CRotation(-90, 0);
                ade.Rotation.XOffset = 0.5f;
                ade.Rotation.YOffset = -0.5f;
                ade.Rotation.Equation = new CSpring();

                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, null, ade, false);
            }



            {

                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 0), new PointF(-1, 0));
                slm.Equation = new CNonLinear();
                ade.Movement = slm;
                ade.Rotation = new CRotation(0, -90);
                ade.Rotation.Equation = new CNonLinear();

                CAnimatedDecorationEffect ade2 = new CAnimatedDecorationEffect();
                CStraightLineMovement slm2 = new CStraightLineMovement(new PointF(0, -1), new PointF(0, 0));
                slm2.Equation = new CNonLinear();
                ade2.Movement = slm2;
                ade2.Rotation = new CRotation(180, 0);
                ade2.Rotation.Equation = new CNonLinear();

                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, ade, ade2, true);
            }


            {

                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 0), new PointF(1, 0));
                slm.Equation = new CNonLinear();
                ade.Movement = slm;
                ade.Rotation = new CRotation(0, -90);
                ade.Rotation.Equation = new CNonLinear();

                CAnimatedDecorationEffect ade2 = new CAnimatedDecorationEffect();
                CStraightLineMovement slm2 = new CStraightLineMovement(new PointF(0, 1), new PointF(0, 0));
                slm2.Equation = new CNonLinear();
                ade2.Movement = slm2;
                ade2.Rotation = new CRotation(180, 0);
                ade2.Rotation.Equation = new CNonLinear();

                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, ade, ade2, true);
            }


            {

                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 0), new PointF(-3, 0));
                slm.StartZ = 1;
                slm.EndZ = 5;
                slm.Equation = new CNonLinear();
                ade.Movement = slm;

                CAnimatedDecorationEffect ade2 = new CAnimatedDecorationEffect();
                CStraightLineMovement slm2 = new CStraightLineMovement(new PointF(3, 0), new PointF(0, 0));
                slm2.Equation = new CNonLinear();
                slm2.StartZ = 5;
                slm2.EndZ = 1;
                ade2.Movement = slm2;

                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, ade, ade2, true);
            }


            {

                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 0), new PointF(3, 0));
                slm.StartZ = 1;
                slm.EndZ = 5;
                slm.Equation = new CNonLinear();
                ade.Movement = slm;

                CAnimatedDecorationEffect ade2 = new CAnimatedDecorationEffect();
                CStraightLineMovement slm2 = new CStraightLineMovement(new PointF(-3, 0), new PointF(0, 0));
                slm2.Equation = new CNonLinear();
                slm2.StartZ = 5;
                slm2.EndZ = 1;
                ade2.Movement = slm2;

                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, ade, ade2, true);
            }

            {

                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 0), new PointF(0, 3));
                slm.StartZ = 1;
                slm.EndZ = 5;
                slm.Equation = new CNonLinear();
                ade.Movement = slm;

                CAnimatedDecorationEffect ade2 = new CAnimatedDecorationEffect();
                CStraightLineMovement slm2 = new CStraightLineMovement(new PointF(0, -3), new PointF(0, 0));
                slm2.Equation = new CNonLinear();
                slm2.StartZ = 5;
                slm2.EndZ = 1;
                ade2.Movement = slm2;

                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, ade, ade2, true);
            }

            {

                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 0), new PointF(0, -3));
                slm.StartZ = 1;
                slm.EndZ = 5;
                slm.Equation = new CNonLinear();
                ade.Movement = slm;

                CAnimatedDecorationEffect ade2 = new CAnimatedDecorationEffect();
                CStraightLineMovement slm2 = new CStraightLineMovement(new PointF(0, 3), new PointF(0, 0));
                slm2.Equation = new CNonLinear();
                slm2.StartZ = 5;
                slm2.EndZ = 1;
                ade2.Movement = slm2;

                mEffectsTable[index++] = new MovementTransitionEffect(0.7f, ade, ade2, true);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(-1, -1), new PointF(0, 0));
                slm.Equation = new CSpring();
                ade.Movement = slm;
                ade.Rotation = new CRotation(-182, 0);
                ade.Rotation.Equation = new CSpring();
                mEffectsTable[index++] = new MovementTransitionEffect(1.5f, null, ade, false);
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(1, -1), new PointF(0, 0));
                slm.Equation = new CSpring();
                ade.Movement = slm;
                ade.Rotation = new CRotation(182, 0);
                ade.Rotation.Equation = new CSpring();
                mEffectsTable[index++] = new MovementTransitionEffect(1.5f, null, ade, false);
            }

            // batman effects
            {
                string filename = CGlobals.GetClipartDirector() + "\\snowfl.wmf";
                BatmanTransitionEffect effect = new BatmanTransitionEffect(0.76f, filename, 1.3f, 1.1f, 0, 90.0f);
                mEffectsTable[index++] = effect;
            }

            {
                string filename = CGlobals.GetClipartDirector() + "\\Cake.png";
                BatmanTransitionEffect effect = new BatmanTransitionEffect(1.5f, filename, 1.0f, 1.0f, 0, 0.0f);
                mEffectsTable[index++] = effect;
            }

            {
                string filename = CGlobals.GetClipartDirector() + "\\Splat1.png";
                BatmanTransitionEffect effect = new BatmanTransitionEffect(1.5f, filename, 1.0f, 1.0f, 0, 0.0f);
                mEffectsTable[index++] = effect;
            }


            {
                string filename = CGlobals.GetClipartDirector() + "\\Flower10.png";
                BatmanTransitionEffect effect = new BatmanTransitionEffect(1.5f, filename, 1.0f, 1.0f, 0, 0.0f);
                mEffectsTable[index++] = effect;
            }


            {
                string filename = CGlobals.GetClipartDirector() + "\\Heart4.png";
                BatmanTransitionEffect effect = new BatmanTransitionEffect(1.5f, filename, 1.0f, 1.0f, 0, 0.0f);
                mEffectsTable[index++] = effect;
            }

            {
                string filename = CGlobals.GetClipartDirector() + "\\Heart3.png";
                BatmanTransitionEffect effect = new BatmanTransitionEffect(1.5f, filename, 1.0f, 1.0f, 0, 0.0f);
                mEffectsTable[index++] = effect;
            }

            {
                string filename = CGlobals.GetClipartDirector() + "\\Pumpkin.png";
                BatmanTransitionEffect effect = new BatmanTransitionEffect(1.5f, filename, 1.0f, 1.0f, 0, 0.0f);
                mEffectsTable[index++] = effect;
            }


            {
                string filename = CGlobals.GetClipartDirector() + "\\Snowman.png";
                BatmanTransitionEffect effect = new BatmanTransitionEffect(1.5f, filename, 1.0f, 1.0f, 0, 0.0f);
                mEffectsTable[index++] = effect;
            }


            {
                string filename = CGlobals.GetClipartDirector() + "\\sun.png";
                BatmanTransitionEffect effect = new BatmanTransitionEffect(0.76f, filename, 1.0f, 1.0f, 0, -90.0f);
                mEffectsTable[index++] = effect;
            }

            DiamondTransitionEffect dteout = new DiamondTransitionEffect(1.5f, 12, true);
            mEffectsTable[index++] = dteout;

            DiamondTransitionEffect dtein = new DiamondTransitionEffect(1.5f, 12, false);
            mEffectsTable[index++] = dtein;

            RectangleTransitionEffect rteout = new RectangleTransitionEffect(1.5f, 12, true);
            mEffectsTable[index++] = rteout;

            RectangleTransitionEffect rtein = new RectangleTransitionEffect(1.5f, 12, false);
            mEffectsTable[index++] = rtein;

            CrossTransitionEffect cteout = new CrossTransitionEffect(1.5f, 12, true);
            mEffectsTable[index++] = cteout;

            CrossTransitionEffect ctein = new CrossTransitionEffect(1.5f, 12, false);
            mEffectsTable[index++] = ctein;

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                ade.Zoom = new CZoom(1, 3);
                ade.Rotation = new CRotation(0, 180);
                ade.Rotation.Equation = new CNonLinear();

                CAnimatedDecorationEffect ade2 = new CAnimatedDecorationEffect();
                ade2.Zoom = new CZoom(3, 1);
                ade2.Rotation = new CRotation(180, 0);
                ade2.Rotation.Equation = new CNonLinear();
                MovementTransitionEffect mte = new MovementTransitionEffect(1.5f, ade, ade2, false);
                mte.DoAlphaBlend = true;
                mEffectsTable[index++] = mte;
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                ade.Zoom = new CZoom(1, 3);
                ade.Rotation = new CRotation(0, -180);
                ade.Rotation.Equation = new CNonLinear();

                CAnimatedDecorationEffect ade2 = new CAnimatedDecorationEffect();
                ade2.Zoom = new CZoom(3, 1);
                ade2.Rotation = new CRotation(-180, 0);
                ade2.Rotation.Equation = new CNonLinear();
                MovementTransitionEffect mte = new MovementTransitionEffect(1.5f, ade, ade2, false);
                mte.DoAlphaBlend = true;
                mEffectsTable[index++] = mte;
            }



            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                ade.Zoom = new CZoom(1, 0.5f);
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 0), new PointF(0, 1));
                ade.Movement = slm;
                ade.Movement.InitialDelay = 0.5f;
                slm.Equation = new CNonLinear();

                CAnimatedDecorationEffect ade2 = new CAnimatedDecorationEffect();
                ade2.Zoom = new CZoom(0.5f, 1);
                ade2.Zoom.InitialDelay = 0.5f;

                MovementTransitionEffect mte = new MovementTransitionEffect(1.5f, ade, ade2, true);
                mEffectsTable[index++] = mte;
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                ade.Zoom = new CZoom(1, 0.5f);
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 0), new PointF(0, -1));
                ade.Movement = slm;
                ade.Movement.InitialDelay = 0.5f;
                slm.Equation = new CNonLinear();

                CAnimatedDecorationEffect ade2 = new CAnimatedDecorationEffect();
                ade2.Zoom = new CZoom(0.5f, 1);
                ade2.Zoom.InitialDelay = 0.5f;

                MovementTransitionEffect mte = new MovementTransitionEffect(1.5f, ade, ade2, true);
                mEffectsTable[index++] = mte;
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                ade.Zoom = new CZoom(1, 0.5f);
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 0), new PointF(-1, 0));
                ade.Movement = slm;
                ade.Movement.InitialDelay = 0.5f;
                slm.Equation = new CNonLinear();

                CAnimatedDecorationEffect ade2 = new CAnimatedDecorationEffect();
                ade2.Zoom = new CZoom(0.5f, 1);
                ade2.Zoom.InitialDelay = 0.5f;

                MovementTransitionEffect mte = new MovementTransitionEffect(1.5f, ade, ade2, true);
                mEffectsTable[index++] = mte;
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                ade.Zoom = new CZoom(1, 0.5f);
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 0), new PointF(1, 0));
                ade.Movement = slm;
                ade.Movement.InitialDelay = 0.5f;
                slm.Equation = new CNonLinear();

                CAnimatedDecorationEffect ade2 = new CAnimatedDecorationEffect();
                ade2.Zoom = new CZoom(0.5f, 1);
                ade2.Zoom.InitialDelay = 0.5f;

                MovementTransitionEffect mte = new MovementTransitionEffect(1.5f, ade, ade2, true);
                mEffectsTable[index++] = mte;
            }


            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 0), new PointF(2f, 2f));
                slm.StartZ = 1;
                slm.EndZ = 5;
                slm.TranslateXYIfZSet = true;
                slm.Equation = new CNonLinear();
                ade.Rotation = new CRotation(0, 360);
                ade.Movement = slm;

                CAnimatedDecorationEffect ade2 = new CAnimatedDecorationEffect();
                CStraightLineMovement slm2 = new CStraightLineMovement(new PointF(-2f, -2f), new PointF(0, 0));
                slm2.StartZ = 5;
                slm2.EndZ = 1;
                slm2.TranslateXYIfZSet = true;
                slm2.Equation = new CNonLinear();
                ade2.Rotation = new CRotation(-360, 0);
                ade2.Movement = slm2;

                MovementTransitionEffect mte = new MovementTransitionEffect(1.5f, ade, ade2, false);
                mte.DoAlphaBlend = true;
                mEffectsTable[index++] = mte;


            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CStraightLineMovement slm = new CStraightLineMovement(new PointF(0, 0), new PointF(-2f, -2f));
                slm.StartZ = 1;
                slm.EndZ = 5;
                slm.TranslateXYIfZSet = true;
                slm.Equation = new CNonLinear();
                ade.Rotation = new CRotation(0, 360);
                ade.Movement = slm;

                CAnimatedDecorationEffect ade2 = new CAnimatedDecorationEffect();
                CStraightLineMovement slm2 = new CStraightLineMovement(new PointF(2f, 2f), new PointF(0, 0));
                slm2.StartZ = 5;
                slm2.EndZ = 1;
                slm2.TranslateXYIfZSet = true;
                slm2.Equation = new CNonLinear();
                ade2.Rotation = new CRotation(-360, 0);
                ade2.Movement = slm2;

                MovementTransitionEffect mte = new MovementTransitionEffect(1.5f, ade, ade2, false);
                mte.DoAlphaBlend = true;
                mEffectsTable[index++] = mte;
            }


            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CVerticalStretch vs = new CVerticalStretch(1, 0);
                vs.Equation = new CNonLinear();
                vs.EndDelay = 0.5f;
                ade.Stretch = vs;

                CAnimatedDecorationEffect ade2 = new CAnimatedDecorationEffect();
                CVerticalStretch vs2 = new CVerticalStretch(0, 1);
                vs2.Equation = new CNonLinear();
                vs2.InitialDelay = 0.5f;
                ade2.Stretch = vs2;
                MovementTransitionEffect mte = new MovementTransitionEffect(0.5f, ade, ade2, false);
                mte.DoAlphaBlend = true;
                mEffectsTable[index++] = mte;

            }


            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CHorizontalStretch hs = new CHorizontalStretch(1, 0);
                hs.Equation = new CNonLinear();
                hs.EndDelay = 0.5f;
                ade.Stretch = hs;

                CAnimatedDecorationEffect ade2 = new CAnimatedDecorationEffect();
                CHorizontalStretch hs2 = new CHorizontalStretch(0, 1);
                hs2.Equation = new CNonLinear();
                hs2.InitialDelay = 0.5f;
                ade2.Stretch = hs2;

                MovementTransitionEffect mte = new MovementTransitionEffect(0.5f, ade, ade2, false);
                mte.DoAlphaBlend = true;
                mEffectsTable[index++] = mte;
            }


            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                ade.Rotation = new CRotation(0, 180);
                ade.Rotation.YOffset = 0.5f;

                CAnimatedDecorationEffect ade2 = new CAnimatedDecorationEffect();
                ade2.Rotation = new CRotation(-180, 0);
                ade2.Rotation.YOffset = 0.5f;

                MovementTransitionEffect mte = new MovementTransitionEffect(0.5f, ade, ade2, false);
                mEffectsTable[index++] = mte;
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                ade.Rotation = new CRotation(0, 180);
                ade.Rotation.YOffset = -0.5f;

                CAnimatedDecorationEffect ade2 = new CAnimatedDecorationEffect();
                ade2.Rotation = new CRotation(-180, 0);
                ade2.Rotation.YOffset = -0.5f;

                MovementTransitionEffect mte = new MovementTransitionEffect(0.5f, ade, ade2, false);
                mEffectsTable[index++] = mte;
            }



            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                ade.Rotation = new CRotation(0, 180);
                ade.Rotation.XOffset = 0.5f;

                CAnimatedDecorationEffect ade2 = new CAnimatedDecorationEffect();
                ade2.Rotation = new CRotation(-180, 0);
                ade2.Rotation.XOffset = 0.5f;

                MovementTransitionEffect mte = new MovementTransitionEffect(0.5f, ade, ade2, false);
                mEffectsTable[index++] = mte;
            }

            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                ade.Rotation = new CRotation(0, 180);
                ade.Rotation.XOffset = -0.5f;

                CAnimatedDecorationEffect ade2 = new CAnimatedDecorationEffect();
                ade2.Rotation = new CRotation(-180, 0);
                ade2.Rotation.XOffset = -0.5f;

                MovementTransitionEffect mte = new MovementTransitionEffect(0.5f, ade, ade2, false);
                mEffectsTable[index++] = mte;
            }


            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CHorizontalStretch hs = new CHorizontalStretch(1, 0);
                hs.Equation = new CNonLinear();
                ade.Stretch = hs;
                ade.Zoom = new CZoom(1, 5);

                MovementTransitionEffect mte = new MovementTransitionEffect(0.5f, ade, null, true);
                mte.DoAlphaBlend = true;
                mEffectsTable[index++] = mte;
            }



            {
                CAnimatedDecorationEffect ade = new CAnimatedDecorationEffect();
                CVerticalStretch vs = new CVerticalStretch(1, 0);
                vs.Equation = new CNonLinear();
                ade.Stretch = vs;
                ade.Zoom = new CZoom(1, 5);

                MovementTransitionEffect mte = new MovementTransitionEffect(0.5f, ade, null, true);
                mte.DoAlphaBlend = true;
                mEffectsTable[index++] = mte;
            }


            {
                CTransitionEffect effect = new FanTransitionEffect(1.5f, 32, 0, false, 6);
                mEffectsTable[index++] = effect;
            }

            {
                CTransitionEffect effect = new FanTransitionEffect(1.5f, 32, 12, false, 8);
                mEffectsTable[index++] = effect;
            }

            {
                CTransitionEffect effect = new FanTransitionEffect(1.5f, 32, 1, true, 6);
                mEffectsTable[index++] = effect;
            }


            for (int j = 0; j < numEffects; j++)
            {
                if (mEffectsTable[j] == null)
                {
                    mEffectsTable[j] = new SimpleAlphaBlendTransitionEffect(1.5f);
                }

                mEffectsTable[j].Index = j;
            }


            // SET THIS LAST because creating the uses an effect in the above table
            mEffectsTable[0] = new RandomTransitionEffect(1.5f);
        }
    } 
}
