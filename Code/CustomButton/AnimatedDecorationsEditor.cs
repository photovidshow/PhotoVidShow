using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using CustomButton;
using System.Collections;

namespace CustomButton
{
    public partial class AnimatedDecorationsEditor : Form
    {
        private CAnimatedDecorationEffect mCurrentEffect = null;
        private CSlide mForSlide = null;
        private MiniPreviewController mPreviewController= null;
        private CAnimatedDecoration mForDecoration = null;
        private bool mRebuilding = false;

        public AnimatedDecorationsEditor(CSlideShow forSlideshow, CSlide forSlide, CAnimatedDecoration forDecoration)
        {
            mForSlide = forSlide;
            mForDecoration = forDecoration;

            InitializeComponent();

            // Setup character delay type
            mCharacterOrderComboBox.Items.Add("First to last");
            mCharacterOrderComboBox.Items.Add("Last to first");
            mCharacterOrderComboBox.Items.Add("Random");
            mCharacterOrderComboBox.SelectedIndex = 0;

            // Movement
            mMovementComboBox.Items.Add("None");
            mMovementComboBox.Items.Add( CStraightLineMovement.MovementString() );
            mMovementSubTypeComboBox.Items.Add(CLinear.EquationString());
            mMovementSubTypeComboBox.Items.Add(CNonLinear.EquationString());
            mMovementSubTypeComboBox.Items.Add(CSpring.EquationString());
            mMovementSubTypeComboBox.Items.Add(CQuickSlow.EquationString());
            mMovementSubTypeComboBox.SelectedIndex = 0;

            // Stretch
            mStretchTypeComboBox.Items.Add("None");
            mStretchTypeComboBox.Items.Add( CVerticalStretch.StretchString() );
            mStretchTypeComboBox.Items.Add(CHorizontalStretch.StretchString());
            mStretchEquationComboBox.Items.Add(CLinear.EquationString());
            mStretchEquationComboBox.Items.Add(CNonLinear.EquationString());
            mStretchEquationComboBox.Items.Add(CSpring.EquationString());
            mStretchEquationComboBox.Items.Add(CQuickSlow.EquationString());
            mStretchEquationComboBox.SelectedIndex = 0;
 
            // Transition
            object[] items = new object[14];
            items[0] = "None"; 
            items[1] = "Simple Alpha Blend";
            items[2] = "Fade Up Transition Effect";
            items[3] = "Fade Down Transition Effect";
            items[4] = "Right Alpha Swipe Transition Effect";
            items[5] = "Left Alpha Swipe Transition Effect";
            items[6] = "White saturate Transition Effect";
            items[7] = "Multi circles out";
            items[8] = "Multi circles in";
            items[9] = "Multi diagonal lines";
            items[10] = "Multi lines up down";
            items[11] = "Split diagonal";
            items[12] = "Split up down";
            items[13] = "Split left right";

            mTransitionEffectsListBox.Items.AddRange(items);


            // Rotation
            mRotationMovementTypeComboBox.Items.Add(CLinear.EquationString());
            mRotationMovementTypeComboBox.Items.Add(CNonLinear.EquationString());
            mRotationMovementTypeComboBox.Items.Add(CSpring.EquationString());
            mRotationMovementTypeComboBox.Items.Add(CQuickSlow.EquationString());
            mRotationMovementTypeComboBox.SelectedIndex = 0;

            // Zoom
           
            // Preview window
            mPreviewController = new MiniPreviewController(
                forSlideshow,
                this.mForSlide, this.mPreviewPictureBox);

            mZoomEquationComboBox.Items.Add(CLinear.EquationString());
            mZoomEquationComboBox.Items.Add(CNonLinear.EquationString());
            mZoomEquationComboBox.Items.Add(CSpring.EquationString());
            mZoomEquationComboBox.Items.Add(CQuickSlow.EquationString());
            mZoomEquationComboBox.SelectedIndex = 1;

            // Length
            items = new object[60];
            for (int i = 1; i <= 60; i++)
            {
                items[i - 1] = (i).ToString();
            }
            mLengthInSecondsCombo.Items.AddRange(items);

            // rebuild effects list

            RebuildEffectsListComboBox("");

        }

        protected override void OnClosed(EventArgs e)
        {
            if (mPreviewController != null)
            {
                mPreviewController.Stop();
            }
        }

        private void mNewButton_Click(object sender, EventArgs e)
        {
            AnimatedDecorationNamer namer = new AnimatedDecorationNamer(AnimatedDecorationNamer.AnimatedDecorationNamerType.Create);
            DialogResult result = namer.ShowDialog();

            if (result == DialogResult.OK)
            {
                string name = namer.EffectName;
                if (CAnimatedDecorationEffectDatabase.GetInstance().EffectExists(name) == true)
                {
                    MessageBox.Show("Effect '" + name + "' already exists", "Effect already exists", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    CAnimatedDecorationEffect effect = new CAnimatedDecorationEffect(name);
                    effect.TemplateOnlyEffect = true;

                    CAnimatedDecorationEffectDatabase.GetInstance().AddEffect(effect);

                    RebuildEffectsListComboBox(name);

                }
            }
        }

        //************************************************************************************************
        private CAnimatedDecorationEffect GetSelectedDecorationMoveEffect()
        {
            if (mMotionInEffectRadio.Checked == true)
            {
                if (mForDecoration.MoveInEffect != null)
                {
                    return mForDecoration.MoveInEffect;
                }
            }
            else
            {
                if (mForDecoration.MoveOutEffect != null)
                {
                    return mForDecoration.MoveOutEffect;
                }
            }

            return null;
        }


        private void RebuildEffectsListComboBox(string current)
        {
            mRebuilding = true;
            try
            {
                mEffectsComboBox.Items.Clear();
                mUVEffectsCombo.Items.Clear();

                List<CAnimatedDecorationEffect> effects = CAnimatedDecorationEffectDatabase.GetInstance().Effects;

                string[] objects = new string[effects.Count + 1];

                int index = 0;

                objects[index++] = "None";

                foreach (CAnimatedDecorationEffect effect in effects)
                {
                    objects[index++] = effect.Name;
                }

                mEffectsComboBox.Items.AddRange(objects);
                mUVEffectsCombo.Items.AddRange(objects);

                if (current != "")
                {
                    mEffectsComboBox.Text = current;
                }
                else if (effects.Count > 0)
                {
                    bool found = false;
                    CAnimatedDecorationEffect currenteffect = null;

                    foreach (string s in mEffectsComboBox.Items)
                    {
                        currenteffect = GetSelectedDecorationMoveEffect();

                        if (currenteffect != null &&
                            s == currenteffect.Name)
                        {
                            mEffectsComboBox.SelectedItem = s;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        mEffectsComboBox.SelectedIndex = 0;
                        mUVEffectsCombo.SelectedIndex = 0;
                    }
                    else
                    {
                        // now uv
                        bool uvfound = false;
                        foreach (string ss in mEffectsComboBox.Items)
                        {
                            string UVeffect = currenteffect.UVAnimatedEffectName;

                            if (UVeffect != null && ss == UVeffect)
                            {
                                mUVEffectsCombo.SelectedItem = ss;
                                uvfound = true;
                                break;
                            }
                        }
                        if (uvfound == false)
                        {
                            mUVEffectsCombo.SelectedIndex = 0;
                        }
                    }
                }

                if (mEffectsComboBox.SelectedIndex == 0)
                {
                    mSettingsTab.Enabled = false;
                }
                else
                {
                    mSettingsTab.Enabled = true;
                }
            }
            finally
            {
                mRebuilding = false;
            }
        }

        private void SetMovementSubTypeFromEquation(CEquation equation, ComboBox combo)
        {
            if (equation is CLinear)
            {
                combo.SelectedIndex = 0;
            }
            else if (equation is CNonLinear)
            {
                combo.SelectedIndex = 1;
            }
            else if (equation is CSpring)
            {
                combo.SelectedIndex = 2;
            }
            else if (equation is CQuickSlow)
            {
                combo.SelectedIndex = 3;
            }
            else
            {
                combo.SelectedIndex = 0;
            }
        }

        /// <summary>
        ///  Populate Controls From effect
        /// </summary>
        /// <param name="effect"></param>
        private void PopulateControlsFromEffect(CAnimatedDecorationEffect effect)
        {
            if (effect == null) return ;

            //  Time
            mLengthInSecondsCombo.Text = effect.LengthInSeconds.ToString();
            mLengthSetToLengthOfDecorationCheckBox.Checked = effect.LengthSetToDecorationLength;

            bool null_movement = true;

            // Movemnt
            if (effect.Movement == null)
            {
                mMovementComboBox.SelectedIndex = 0;
                mMovementSubTypeComboBox.Enabled = false;
                mMovementSubTypeComboBox.Text ="";

            }
            else if (effect.Movement is CStraightLineMovement)
            {
                CStraightLineMovement slm = effect.Movement as CStraightLineMovement; 

                this.mMovementComboBox.SelectedIndex = 1;
                null_movement = false;
                PointF startPoint = slm.StartPoint ;
                mMovementStartPointXTextBox.Text = startPoint.X.ToString();
                mMovementStartPointYTextBox.Text = startPoint.Y.ToString();
                mMovementStartPointZTextBox.Text = slm.StartZ.ToString();


                PointF endPoint = slm.EndPoint;
                mMovementEndPointXTextBox.Text = endPoint.X.ToString();
                mMovementEndPointYTextBox.Text = endPoint.Y.ToString();
                mMovementEndPointZTextBox.Text = slm.EndZ.ToString() ;

                mTranslateXYIfZSetCheckBox.Checked = slm.TranslateXYIfZSet;
            }

            if (null_movement == false)
            {
                this.mMovementEndPointXTextBox.Enabled = true;
                this.mMovementEndPointYTextBox.Enabled = true;
                this.mMovementEndPointZTextBox.Enabled = true;
                this.mMovementStartPointXTextBox.Enabled = true;
                this.mMovementStartPointYTextBox.Enabled = true;
                this.mMovementStartPointZTextBox.Enabled = true;
                this.mTranslateXYIfZSetCheckBox.Enabled = true;
                this.mMovementSubTypeComboBox.Enabled = true;

                SetMovementSubTypeFromEquation(effect.Movement.Equation, mMovementSubTypeComboBox);
            
                float v = mCurrentEffect.Movement.InitialDelay ;
                v= CGlobals.DeltaClamp(v);
                this.mMovementInitialDelayTrackBar.Value = (int)(v * 100);

                v = mCurrentEffect.Movement.EndDelay;
                v = CGlobals.DeltaClamp(v);
                v = 1 - v;
                this.mMovementEndDelayTrackBar.Value = (int)(v * 100);

                ReDrawMovementDelayTextBoxes();
            }

            // Stretch
            if (effect.Stretch == null)
            {
                mStretchTypeComboBox.SelectedIndex = 0;
                mStretchEquationComboBox.Enabled = false;
                mStretchEquationComboBox.Text ="";

            }
            else if (effect.Stretch is CVerticalStretch)
            {
                mStretchTypeComboBox.SelectedIndex = 1;
            } 
            else if (effect.Stretch is CHorizontalStretch)
            {
                mStretchTypeComboBox.SelectedIndex = 2;
            }

            if (effect.Stretch != null )
            {
                mStretchStartMultiplier.Text = effect.Stretch.StartMultiplier.ToString();
                mStretchEndMultiplier.Text = effect.Stretch.EndMultiplier.ToString();
                mStretchOffsetMultiplier.Text = "0"; /// SRG TODO
                mStretchEquationComboBox.Enabled = true;

                SetMovementSubTypeFromEquation(effect.Stretch.Equation, mStretchEquationComboBox);

                float v = mCurrentEffect.Stretch.InitialDelay;
                v = CGlobals.DeltaClamp(v);
                mStretchInitialDelayTrackBar.Value = (int)(v * 100);

                v = mCurrentEffect.Stretch.EndDelay;
                v = CGlobals.DeltaClamp(v);
                v = 1 - v;
                mStretchEndDelayTrackBar.Value = (int)(v * 100);

                ReDrawMovementDelayTextBoxes();
            }
         
            int index=0;

            // Transition  
            if (effect.TransitionEffect != null)
            {
                if (effect.TransitionEffect is SimpleAlphaBlendTransitionEffect) index = 1;
                else if (effect.TransitionEffect is FadeUpTransitionEffect) index = 2;
                else if (effect.TransitionEffect is FadeDownTransitionEffect) index = 3;
                else if (effect.TransitionEffect is AlphaSwipTransitionEffect)
                {
                    AlphaSwipTransitionEffect te = effect.TransitionEffect as AlphaSwipTransitionEffect;
                    if (te.Direction == AlphaSwipTransitionEffect.SwipeDirection.RIGHT) index = 4;
                    if (te.Direction == AlphaSwipTransitionEffect.SwipeDirection.LEFT) index = 5;
                }
                else if (effect.TransitionEffect is MiddleSaturateToColorTransitionEffect) index = 6;
                else if (effect.TransitionEffect is MultiCirclesInOutTransitionEffect)
                {
                    MultiCirclesInOutTransitionEffect mce = effect.TransitionEffect as MultiCirclesInOutTransitionEffect;
                    if (mce.OutEffect == true) index = 7;
                    if (mce.OutEffect == false) index = 8;
                }

                else if (effect.TransitionEffect is MultipleLinesDiagonalTransitionEffect) index = 9;
                else if (effect.TransitionEffect is MultipleLinesUpDownTransitionEffect) index = 10;
                else if (effect.TransitionEffect is MiddleSplitDiagonalTransitionEffect) index = 11;
                else if (effect.TransitionEffect is MiddleSplitUpDownTransition) index = 12;
                else if (effect.TransitionEffect is MiddleSplitLeftRightTransition) index = 13;

                float v = mCurrentEffect.TransitionEffect.InitialDelay;
                v = CGlobals.DeltaClamp(v);
                mTransitionInitialDelayTrackBar.Value = (int)(v * 100);

                v = mCurrentEffect.TransitionEffect.EndDelay;
                v = CGlobals.DeltaClamp(v);
                v = 1 - v;
                mTransitionEndDelayTrackBar.Value = (int)(v * 100);

                ReDrawTransitionDelayTextBoxes();

            }

            this.mTransitionEffectsListBox.SelectedIndex = index;

            // Rotation
            if (effect.Rotation != null)
            {
                mStartRotationTextBox.Text = effect.Rotation.StartRotation.ToString();
                mEndRotationTextBox.Text = effect.Rotation.EndRotation.ToString();
                mRotationXOffsetTextBox.Text = effect.Rotation.XOffset.ToString();
                mRotationYOffsetTextBox.Text = effect.Rotation.YOffset.ToString();
      
                SetMovementSubTypeFromEquation(effect.Rotation.Equation, this.mRotationMovementTypeComboBox);

                float v = mCurrentEffect.Rotation.InitialDelay;
                v = CGlobals.DeltaClamp(v);
                mRotationInitialDelayTrackBar.Value = (int)(v * 100);

                v = mCurrentEffect.Rotation.EndDelay;
                v = CGlobals.DeltaClamp(v);
                v = 1 - v;
                mRotationEndDelayTrackBar.Value = (int)(v * 100);

                ReDrawRotationDelayTextBoxes();
            }

            // Zoom
            if (effect.Zoom != null)
            {
                mZoomStartTextBox.Text = effect.Zoom.StartZoom.ToString();
                mZoomEndTextBox.Text = effect.Zoom.EndZoom.ToString();

                SetMovementSubTypeFromEquation(effect.Zoom.Equation, mZoomEquationComboBox);

                float v = mCurrentEffect.Zoom.InitialDelay;
                v = CGlobals.DeltaClamp(v);
                mZoomInitialDelayTrackBar.Value = (int)(v * 100);

                v = mCurrentEffect.Zoom.EndDelay;
                v = CGlobals.DeltaClamp(v);
                v = 1 - v;
                mZoomEndDelayTrackBar.Value = (int)(v * 100);

                ReDrawZoomDelayTextBoxes();
            }

            // Character delay
            if (effect.HasCharacterTimeDelay)
            {
                mCharacterDelayCheckBox.Checked = true;
                mCharacterOrderComboBox.SelectedIndex = ((int)effect.CharacterOrder);
                mCharacterDelayTextBox.Text = effect.CharacerTimeDelayInSeconds.ToString();
            }
            else
            {
                mCharacterDelayCheckBox.Checked = false;
            }

            if (mRebuilding == false)
            {
                if (string.IsNullOrEmpty(effect.UVAnimatedEffectName) == false)
                {
                    mUVEffectsCombo.SelectedItem = effect.UVAnimatedEffectName;
                }
                else
                {
                    mUVEffectsCombo.SelectedIndex = 0;
                }
            }
        }

        private void mEffectsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            mCurrentEffect = CAnimatedDecorationEffectDatabase.GetInstance().Get( mEffectsComboBox.SelectedItem.ToString() );
            PopulateControlsFromEffect(mCurrentEffect);

            if (mMotionInEffectRadio.Checked == true)
            {
                mForDecoration.MoveInEffect = mCurrentEffect;
            }
            else
            {
                mForDecoration.MoveOutEffect = mCurrentEffect;
            }

            if (mEffectsComboBox.SelectedIndex == 0)
            {
                mSettingsTab.Enabled = false;
            }
            else
            {
                mSettingsTab.Enabled = true;
            }
        }

        private void mUVEffectsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mRebuilding == true)
            {
                return;
            }

            if (mUVEffectsCombo.SelectedIndex == 0)
            {
                mCurrentEffect.UVAnimatedEffectName = null;
                return;
            }

            string newName = this.mUVEffectsCombo.SelectedItem.ToString();

            //
            // Prevent recusion
            //
            if (mCurrentEffect.Name != newName)
            {
                mCurrentEffect.UVAnimatedEffectName = newName;
            }
            else
            {
                mCurrentEffect.UVAnimatedEffectName = null;
            }
        }

        private bool ConvertAndCheckPointF(string sx, string sy, string sz, ref PointF result, ref float resultZ)
        {
            try
            {
                float x = float.Parse(sx);
                if (x < -100 || x > 100) return false;
                float y = float.Parse(sy);
                if (y < -100 || y > 100) return false;
                float z = float.Parse(sz);
                result = new PointF(x, y);
                resultZ = z;
            }
            catch
            {
                return false;
            }
            return true;
        }

        private CEquation GetEquationFromString(string selected)
        {
            if (selected == CLinear.EquationString())
            {
                return new CLinear();
            }

            if (selected == CNonLinear.EquationString())
            {
                return new CNonLinear();
            }

            if (selected == CSpring.EquationString())
            {
                return new CSpring();
            }

            if (selected == CQuickSlow.EquationString())
            {
                return new CQuickSlow();
            }

            return new CLinear();
        }

        // ********************************************************************************
        //
        // MOVEMENT 
        //
        // ********************************************************************************
        private void SetStraightLineMovementFromControls()
        {
            if (mCurrentEffect == null || mCurrentEffect.Movement == null ) return;
            CStraightLineMovement slm = mCurrentEffect.Movement as CStraightLineMovement;
            if (slm != null)
            {
                PointF sp = slm.StartPoint;
                float sz = slm.StartZ;
                if (ConvertAndCheckPointF(mMovementStartPointXTextBox.Text, mMovementStartPointYTextBox.Text, mMovementStartPointZTextBox.Text, ref sp, ref sz ))
                {
                    slm.StartPoint = sp;
                    slm.StartZ = sz;
                }
                else
                {
                    mMovementStartPointXTextBox.Text = sp.X.ToString();
                    mMovementStartPointYTextBox.Text = sp.Y.ToString();
                    mMovementStartPointZTextBox.Text = sz.ToString();
                }

                

                PointF ep = slm.EndPoint;
                float ez = slm.EndZ;
                if (ConvertAndCheckPointF(mMovementEndPointXTextBox.Text, mMovementEndPointYTextBox.Text, mMovementEndPointZTextBox.Text, ref ep, ref ez))
                {
                    slm.EndPoint = ep;
                    slm.EndZ = ez;
                }
                else
                {
                    mMovementEndPointXTextBox.Text = ep.X.ToString();
                    mMovementEndPointYTextBox.Text = ep.Y.ToString();
                    mMovementEndPointZTextBox.Text = ez.ToString();
                }
            }

            UpdateLinearMoveRateTextBoxes();
        }

        // **************************************************************************************************
        private void SetMovementSubTypeFromControls()
        {
            if (mCurrentEffect == null || mCurrentEffect.Movement == null ) return;
          
            mCurrentEffect.Movement.Equation = GetEquationFromString(this.mMovementSubTypeComboBox.Text);


            UpdateLinearMoveRateTextBoxes();
        }

        // **************************************************************************************************
        private void UpdateLinearMoveRateTextBoxes()
        {
            if (mCurrentEffect.Movement.Equation == null ||
                mCurrentEffect.Movement.Equation is CLinear == false)
            {
                mMovementXRateTextBox.Text = "";
                mMovementXRateTextBox.Enabled = false;

                mMovementYRateTextBox.Text = "";
                mMovementYRateTextBox.Enabled = false;

                mMovementZRateTextBox.Text = "";
                mMovementZRateTextBox.Enabled = false;
            }
            else
            {
                CStraightLineMovement slm = mCurrentEffect.Movement as CStraightLineMovement;

                float xr=0;
                float yr=0;
                float zr=0;

                CalculateLinearMovementSpeed(slm, ref xr, ref yr, ref zr);

                mMovementXRateTextBox.Text = xr.ToString();
                mMovementXRateTextBox.Enabled = true;

                mMovementYRateTextBox.Text = yr.ToString();
                mMovementYRateTextBox.Enabled = true;

                mMovementZRateTextBox.Text = zr.ToString();
                mMovementZRateTextBox.Enabled = true;

            }

        }


        // **************************************************************************************************************
        private void CalculateLinearMovementSpeed(CStraightLineMovement slm, ref float xr, ref float yr, ref float zr)
        {
            float t= mCurrentEffect.LengthInSeconds - slm.InitialDelay - slm.EndDelay;

            if (t == 0) return;

            xr = ((slm.EndPoint.X - slm.StartPoint.X) ) / t;
            yr = ((slm.EndPoint.Y - slm.StartPoint.Y)) / t;
            zr = ((slm.EndZ - slm.StartZ)) / t;

        }

        private void mMovementStartPointXTextBox_Leave(object sender, EventArgs e)
        {
            SetStraightLineMovementFromControls();
        }

        private void mMovementStartPointYTextBox_Leave(object sender, EventArgs e)
        {
            SetStraightLineMovementFromControls();
        }

        private void mMovementStartPointZTextBox_Leave(object sender, EventArgs e)
        {
            SetStraightLineMovementFromControls();
        }   


        private void mPreviewButton_Click(object sender, EventArgs e)
        {

        }

        private void mMovementComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mCurrentEffect == null) return;

            string selected = this.mMovementComboBox.Text;

            if (mCurrentEffect.Movement != null)
            {
                if (selected == mCurrentEffect.Movement.ToMovementString())
                {
                    return;
                }
            }
          
            if (selected == "None")
            {
                mCurrentEffect.Movement = null;
                this.mMovementEndPointXTextBox.Enabled = false;
                this.mMovementEndPointYTextBox.Enabled = false;
                this.mMovementEndPointZTextBox.Enabled = false;
                this.mMovementStartPointXTextBox.Enabled = false;
                this.mMovementStartPointYTextBox.Enabled = false;
                this.mMovementStartPointZTextBox.Enabled = false;
                this.mTranslateXYIfZSetCheckBox.Enabled = false;
                this.mMovementSubTypeComboBox.Enabled = false;
                return;
            }

            this.mMovementEndPointXTextBox.Enabled = true;
            this.mMovementEndPointYTextBox.Enabled = true;
            this.mMovementEndPointZTextBox.Enabled = true;
            this.mMovementStartPointXTextBox.Enabled = true;
            this.mMovementStartPointYTextBox.Enabled = true;
            this.mMovementStartPointZTextBox.Enabled = true;
            this.mTranslateXYIfZSetCheckBox.Enabled = true;
            this.mMovementSubTypeComboBox.Enabled = true;

            if (selected == CStraightLineMovement.MovementString())
            {
                mCurrentEffect.Movement = new CStraightLineMovement();
                SetStraightLineMovementFromControls();
                SetMovementSubTypeFromControls();
            }
        }

        private void mMovementSubTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetMovementSubTypeFromControls();
        }

        private void mTranslateXYIfZSetCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mCurrentEffect != null && mCurrentEffect.Movement != null)
            {
                CStraightLineMovement slm = mCurrentEffect.Movement as CStraightLineMovement ;
                if (slm != null)
                {
                    slm.TranslateXYIfZSet = mTranslateXYIfZSetCheckBox.Checked;
                }
            }
        }

        private void mMovementEndPointXTextBox_Leave(object sender, EventArgs e)
        {
            SetStraightLineMovementFromControls();
        }

        private void mMovementEndPointYTextBox_Leave(object sender, EventArgs e)
        {
            SetStraightLineMovementFromControls();
        }

        private void mMovementEndPointZTextBox_Leave(object sender, EventArgs e)
        {
            SetStraightLineMovementFromControls();
        }

        private void mLengthInSecondsCombo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (mCurrentEffect == null) return;

            try
            {
                if (mLengthInSecondsCombo.SelectedItem != null)
                {
                    this.mCurrentEffect.LengthInSeconds = float.Parse(mLengthInSecondsCombo.SelectedItem.ToString());
                }
                else
                {
                    this.mCurrentEffect.LengthInSeconds = float.Parse(mLengthInSecondsCombo.Text);
                }
            }
            catch
            {
                mLengthInSecondsCombo.Text = mCurrentEffect.LengthInSeconds.ToString();
            }

            ReDrawMovementDelayTextBoxes();
        }

        private void mLengthInSecondsCombo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void mLengthInSecondsCombo_Leave(object sender, EventArgs e)
        {
            mLengthInSecondsCombo_SelectionChangeCommitted(sender, e);
        }

        private void mMovementInitialDelayTrackBar_Scroll(object sender, EventArgs e)
        {
            if (mMovementInitialDelayTrackBar.Value >
                mMovementEndDelayTrackBar.Value)
            {
                mMovementInitialDelayTrackBar.Value = mMovementEndDelayTrackBar.Value;
            }

            ReCalcMovementDelayTimes();
        }

        private void mMovementEndDelayTrackBar_Scroll(object sender, EventArgs e)
        {
            if (mMovementEndDelayTrackBar.Value <
               mMovementInitialDelayTrackBar.Value)
            {
                mMovementEndDelayTrackBar.Value = mMovementInitialDelayTrackBar.Value;
            }

            ReCalcMovementDelayTimes();
        }

        private void ReCalcMovementDelayTimes()
        {
            if (mCurrentEffect == null || mCurrentEffect.Movement == null ) return;

            float start = mMovementInitialDelayTrackBar.Value;
            float end = mMovementEndDelayTrackBar.Value;

            float effectLength = mCurrentEffect.LengthInSeconds;

            start /= 100;
            end /= 100;
            start = CGlobals.DeltaClamp(start);
            end = CGlobals.DeltaClamp(end);
            end = 1 - end;

            mCurrentEffect.Movement.InitialDelay = start;
            mCurrentEffect.Movement.EndDelay = end;

            ReDrawMovementDelayTextBoxes();
        }

        private void ReDrawMovementDelayTextBoxes()
        {
            if (mCurrentEffect == null || mCurrentEffect.Movement == null) return;

            float s = (mCurrentEffect.Movement.InitialDelay * mCurrentEffect.LengthInSeconds) + 0.0499f;
            float e = (mCurrentEffect.Movement.EndDelay * mCurrentEffect.LengthInSeconds) - 0.0499f;

            mMovementInitialDelayTextBox.Text = System.String.Format("{0:f1}",s );
            mMovementEndDelayTextBox.Text = System.String.Format("{0:f1}", e );
        }

        // ********************************************************************************
        private void MovementRateChanged()
        {

            if (mCurrentEffect.Movement == null) return;
            CStraightLineMovement slm = mCurrentEffect.Movement as CStraightLineMovement;
            if (slm == null) return;
            if (mCurrentEffect.Movement.Equation is CLinear == false) return;

            float t = mCurrentEffect.LengthInSeconds - slm.InitialDelay - slm.EndDelay;

            try
            {
                float xr = float.Parse(mMovementXRateTextBox.Text);
                mMovementEndPointXTextBox.Text = ((t * xr)+ slm.StartPoint.X).ToString();
            }
            catch
            {
            }

            try
            {
                float yr = float.Parse(mMovementYRateTextBox.Text);
                mMovementEndPointYTextBox.Text = ((t * yr)+ slm.StartPoint.Y).ToString();
            }
            catch
            {
            }


            try
            {
                float zr = float.Parse(mMovementZRateTextBox.Text);
                mMovementEndPointZTextBox.Text = ((t * zr)+slm.StartZ).ToString();
            }
            catch
            {
            }

            SetStraightLineMovementFromControls();
        }


        // ********************************************************************************
        private void mMovementXRateTextBox_Leave(object sender, EventArgs e)
        {
            MovementRateChanged();
        }


        // ********************************************************************************
        private void mMovementYRateTextBox_Leave(object sender, EventArgs e)
        {
            MovementRateChanged();
        }


        // ********************************************************************************
        private void mMovementZRateTextBox_Leave(object sender, EventArgs e)
        {
            MovementRateChanged();
        }



        // ********************************************************************************
        //
        // TRANSITION 
        //
        // ********************************************************************************
        private void mTransitionInitialDelayTrackBar_Scroll(object sender, EventArgs e)
        {
            if (mTransitionInitialDelayTrackBar.Value >
                mTransitionEndDelayTrackBar.Value)
            {
                mTransitionInitialDelayTrackBar.Value = mTransitionEndDelayTrackBar.Value;
            }

            ReCalcTransitionDelayTimes();
        
        }

        private void mTransitionEndDelayTrackBar_Scroll(object sender, EventArgs e)
        {
            if (mTransitionEndDelayTrackBar.Value <
                mTransitionInitialDelayTrackBar.Value)
            {
                mTransitionEndDelayTrackBar.Value = mTransitionInitialDelayTrackBar.Value;
            }

            ReCalcTransitionDelayTimes();
        }

        private void ReCalcTransitionDelayTimes()
        {
            if (mCurrentEffect == null || mCurrentEffect.TransitionEffect == null) return;

            float start =this.mTransitionInitialDelayTrackBar.Value;
            float end = mTransitionEndDelayTrackBar.Value;

            float effectLength = mCurrentEffect.LengthInSeconds;

            start /= 100;
            end /= 100;
            start = CGlobals.DeltaClamp(start);
            end = CGlobals.DeltaClamp(end);
            end = 1 - end;

            mCurrentEffect.TransitionEffect.InitialDelay = start;
            mCurrentEffect.TransitionEffect.EndDelay = end;

            ReDrawTransitionDelayTextBoxes();
        }

        private void ReDrawTransitionDelayTextBoxes()
        {
            if (mCurrentEffect == null || mCurrentEffect.TransitionEffect == null) return;

            float s = (mCurrentEffect.TransitionEffect.InitialDelay * mCurrentEffect.LengthInSeconds) + 0.0499f;
            float e = (mCurrentEffect.TransitionEffect.EndDelay * mCurrentEffect.LengthInSeconds) - 0.0499f;

            mTransitionInitialDelayTextBox.Text = System.String.Format("{0:f1}", s);
            mTransitionEndDelayTextBox.Text = System.String.Format("{0:f1}", e);
        }


        private void mTransitionEffectsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool changed = false;

            switch (mTransitionEffectsListBox.SelectedIndex)
            {
                case 1:
                    {
                        if (! (this.mCurrentEffect.TransitionEffect is SimpleAlphaBlendTransitionEffect))
                        {
                            this.mCurrentEffect.TransitionEffect = new SimpleAlphaBlendTransitionEffect(1.5f);
                            changed = true;
                        }
                        break;
                    }
                case 2:
                    {
                        if (!(this.mCurrentEffect.TransitionEffect is FadeUpTransitionEffect))
                        {
                            this.mCurrentEffect.TransitionEffect = new FadeUpTransitionEffect(1.5f, 32);
                            changed = true;
                        }
                        break;
                    }
                case 3:
                    {
                        if (!(this.mCurrentEffect.TransitionEffect is FadeDownTransitionEffect))
                        {
                            this.mCurrentEffect.TransitionEffect = new FadeDownTransitionEffect(1.5f, 32);
                            changed = true;
                        }
                        break;
                    }
                case 4:
                    {
                        bool same = false;
                        AlphaSwipTransitionEffect asw = this.mCurrentEffect.TransitionEffect as AlphaSwipTransitionEffect;
                        if ( asw != null)
                        {
                            if (asw.Direction == AlphaSwipTransitionEffect.SwipeDirection.RIGHT)
                            {
                                same = true;
                            }
                        }

                        if (!same)
                        {
                            this.mCurrentEffect.TransitionEffect = new AlphaSwipTransitionEffect(1.5f, AlphaSwipTransitionEffect.SwipeDirection.RIGHT, 2);
                            changed = true;
                        }
                        break;
                    }
                case 5:
                    {
                        bool same = false;
                        AlphaSwipTransitionEffect asw = this.mCurrentEffect.TransitionEffect as AlphaSwipTransitionEffect;
                        if (asw != null)
                        {
                            if (asw.Direction == AlphaSwipTransitionEffect.SwipeDirection.LEFT)
                            {
                                same = true;
                            }
                        }

                        if (!same)
                        {
                            this.mCurrentEffect.TransitionEffect = new AlphaSwipTransitionEffect(1.5f, AlphaSwipTransitionEffect.SwipeDirection.LEFT, 2);
                            changed = true;
                        }

                        break;
                    }
                case 6:
                    {
                        if (!(this.mCurrentEffect.TransitionEffect is MiddleSaturateToColorTransitionEffect))
                        {
                            this.mCurrentEffect.TransitionEffect = new MiddleSaturateToColorTransitionEffect(Color.White, 1.5f);
                            changed = true;
                        }
                        break;
                    }

                case 7:
                    {
                        MultiCirclesInOutTransitionEffect mce = this.mCurrentEffect.TransitionEffect as MultiCirclesInOutTransitionEffect;
                        if (mce == null )
                        {
                            this.mCurrentEffect.TransitionEffect = new MultiCirclesInOutTransitionEffect(true, 10, 1,1,1.5f, 10 ) ;
                            changed = true;
                        }
                        break;
                    }

                case 8:
                    {
                        MultiCirclesInOutTransitionEffect mce = this.mCurrentEffect.TransitionEffect as MultiCirclesInOutTransitionEffect;
                        if (mce == null)
                        {
                            this.mCurrentEffect.TransitionEffect = new MultiCirclesInOutTransitionEffect(false, 5, 1, 1, 1.5f, 10);
                            changed = true;
                        }
                        break;
                    }
                case 9:
                    {
                        MultipleLinesDiagonalTransitionEffect mld = this.mCurrentEffect.TransitionEffect as MultipleLinesDiagonalTransitionEffect;
                        if (mld == null)
                        {
                            this.mCurrentEffect.TransitionEffect = new MultipleLinesDiagonalTransitionEffect(10, false, 1.5f, 10 );
                            changed = true;
                        }
                        break;
                    }

                case 10:
                    {
                        MultipleLinesUpDownTransitionEffect mld = this.mCurrentEffect.TransitionEffect as MultipleLinesUpDownTransitionEffect;
                        if (mld == null)
                        {
                            this.mCurrentEffect.TransitionEffect = new MultipleLinesUpDownTransitionEffect(10, false, 1.5f, 10);
                            changed = true;
                        }
                        break;
                    }

                case 11:
                    {
                        MiddleSplitDiagonalTransitionEffect splitd = this.mCurrentEffect.TransitionEffect as MiddleSplitDiagonalTransitionEffect;
                        if (splitd == null)
                        {
                            this.mCurrentEffect.TransitionEffect = new MiddleSplitDiagonalTransitionEffect(false, true, 1.5f, 10);
                            changed = true;
                        }
                        break;
                    }

                case 12:
                    {
                        MiddleSplitUpDownTransition splitd = this.mCurrentEffect.TransitionEffect as MiddleSplitUpDownTransition;
                        if (splitd == null)
                        {
                            this.mCurrentEffect.TransitionEffect = new MiddleSplitUpDownTransition(false, 1.5f, 10);
                            changed = true;
                        }
                        break;
                    }

                case 13:
                    {
                        MiddleSplitLeftRightTransition splitd = this.mCurrentEffect.TransitionEffect as MiddleSplitLeftRightTransition;
                        if (splitd == null)
                        {
                            this.mCurrentEffect.TransitionEffect = new MiddleSplitLeftRightTransition(false, 1.5f, 10);
                            changed = true;
                        }
                        break;
                    }
                default:
                    {
                        this.mCurrentEffect.TransitionEffect = null;
                        changed = true;
                        break;
                    }
            }

            if (changed)
            {
                ReCalcTransitionDelayTimes();
            }
        }

        // ********************************************************************************
        //
        // ROTATION 
        //
        // ********************************************************************************
        private void mRotationInitialDelayTrackBar_Scroll(object sender, EventArgs e)
        {
            if (mRotationInitialDelayTrackBar.Value >
                mRotationEndDelayTrackBar.Value)
            {
                mRotationInitialDelayTrackBar.Value = mRotationEndDelayTrackBar.Value;
            }

            ReCalcRotationDelayTimes();
        }

        private void mRotationEndDelayTrackBar_Scroll(object sender, EventArgs e)
        {
            if (mRotationEndDelayTrackBar.Value <
                mRotationInitialDelayTrackBar.Value)
            {
                mRotationEndDelayTrackBar.Value = mRotationInitialDelayTrackBar.Value;
            }

            ReCalcRotationDelayTimes();
        }


        private void ReCalcRotationDelayTimes()
        {
            if (mCurrentEffect == null || mCurrentEffect.Rotation == null) return;

            float start = this.mRotationInitialDelayTrackBar.Value;
            float end = mRotationEndDelayTrackBar.Value;

            float effectLength = mCurrentEffect.LengthInSeconds;

            start /= 100;
            end /= 100;
            start = CGlobals.DeltaClamp(start);
            end = CGlobals.DeltaClamp(end);
            end = 1 - end;

            mCurrentEffect.Rotation.InitialDelay = start;
            mCurrentEffect.Rotation.EndDelay = end;

            ReDrawRotationDelayTextBoxes();
        }

        private void ReDrawRotationDelayTextBoxes()
        {
            if (mCurrentEffect == null || mCurrentEffect.Rotation == null) return;

            float s = (mCurrentEffect.Rotation.InitialDelay * mCurrentEffect.LengthInSeconds) + 0.0499f;
            float e = (mCurrentEffect.Rotation.EndDelay * mCurrentEffect.LengthInSeconds) - 0.0499f;

            mRotationInitialDelayTextBox.Text = System.String.Format("{0:f1}", s);
            mRotationEndDelayTextBox.Text = System.String.Format("{0:f1}", e);
        }

        private bool ConvertAndCheckFloat(string r, ref float result)
        {
            try
            {
                float x = float.Parse(r);
                result = x;
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void SetRotationFromControls()
        {
            if (mCurrentEffect == null || mCurrentEffect.Rotation == null) return;

            float sr = mCurrentEffect.Rotation.StartRotation;
            if (ConvertAndCheckFloat(mStartRotationTextBox.Text, ref sr))
            {
                mCurrentEffect.Rotation.StartRotation = sr;
            }
            else
            {
                mStartRotationTextBox.Text = sr.ToString();
            }

            float er = mCurrentEffect.Rotation.EndRotation;
            if (ConvertAndCheckFloat(mEndRotationTextBox.Text, ref er))
            {
                mCurrentEffect.Rotation.EndRotation = er;
            }
            else
            {
                mEndRotationTextBox.Text = er.ToString();
            }

            PointF offset = new PointF(0, 0);
            float z = 0;
            if (ConvertAndCheckPointF(mRotationXOffsetTextBox.Text,
                                       mRotationYOffsetTextBox.Text,
                                       "0",
                                       ref offset,
                                       ref z))
            {
                mCurrentEffect.Rotation.XOffset = offset.X;
                mCurrentEffect.Rotation.YOffset = offset.Y;
            }
            else
            {
                mRotationXOffsetTextBox.Text = mCurrentEffect.Rotation.XOffset.ToString();
                mRotationYOffsetTextBox.Text = mCurrentEffect.Rotation.YOffset.ToString();
            }
        }

        private void mStartRotationTextBox_Leave(object sender, EventArgs e)
        {
            SetRotationFromControls();
        }

        private void mEndRotationTextBox_Leave(object sender, EventArgs e)
        {
            SetRotationFromControls();
        }

        private void mRotationXOffsetTextBox_Leave(object sender, EventArgs e)
        {
            SetRotationFromControls();
        }

        private void mRotationYOffsetTextBox_Leave(object sender, EventArgs e)
        {
            SetRotationFromControls();
        }

        private void mRotationMovementTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetRoationSubtypeFromControls();
        }

        private void SetRoationSubtypeFromControls()
        {
            if (mCurrentEffect == null || mCurrentEffect.Rotation == null) return;

            mCurrentEffect.Rotation.Equation = GetEquationFromString(this.mRotationMovementTypeComboBox.Text);
        }

        // ********************************************************************************
        //
        // ZOOM 
        //
        // ********************************************************************************
        private void mZoomStartTextBox_Leave(object sender, EventArgs e)
        {
            SetZoomFromControls();
        }

        private void mZoomEndTextBox_Leave(object sender, EventArgs e)
        {
            SetZoomFromControls();
        }

        private void SetZoomFromControls()
        {
            if (mCurrentEffect == null || mCurrentEffect.Zoom == null) return;

            float sz = mCurrentEffect.Zoom.StartZoom;
            if (ConvertAndCheckFloat(mZoomStartTextBox.Text, ref sz))
            {
                mCurrentEffect.Zoom.StartZoom = sz;
            }
            else
            {
                mZoomStartTextBox.Text = sz.ToString();
            }

            float ez = mCurrentEffect.Zoom.EndZoom;
            if (ConvertAndCheckFloat(mZoomEndTextBox.Text, ref ez))
            {
                mCurrentEffect.Zoom.EndZoom = ez;
            }
            else
            {
                mZoomEndTextBox.Text = ez.ToString();
            }
        }

        private void ReDrawZoomDelayTextBoxes()
        {
            if (mCurrentEffect == null || mCurrentEffect.Zoom == null) return;

            float s = (mCurrentEffect.Zoom.InitialDelay * mCurrentEffect.LengthInSeconds) + 0.0499f;
            float e = (mCurrentEffect.Zoom.EndDelay * mCurrentEffect.LengthInSeconds) - 0.0499f;

            mZoomInitialDelayTextBox.Text = System.String.Format("{0:f1}", s);
            mZoomEndDelayTextBox.Text = System.String.Format("{0:f1}", e);
        }

        private void mZoomInitialDelayTrackBar_Scroll(object sender, EventArgs e)
        {
            if (mZoomInitialDelayTrackBar.Value >
                mZoomEndDelayTrackBar.Value)
            {
                mZoomInitialDelayTrackBar.Value = mZoomEndDelayTrackBar.Value;
            }

            ReCalcZoomDelayTimes();
        }

        private void mZoomEndDelayTrackBar_Scroll(object sender, EventArgs e)
        {
            if (mZoomEndDelayTrackBar.Value <
               mZoomInitialDelayTrackBar.Value)
            {
                mZoomEndDelayTrackBar.Value = mZoomInitialDelayTrackBar.Value;
            }

            ReCalcZoomDelayTimes();
        }

        private void ReCalcZoomDelayTimes()
        {
            if (mCurrentEffect == null || mCurrentEffect.Zoom == null) return;

            float start = this.mZoomInitialDelayTrackBar.Value;
            float end = mZoomEndDelayTrackBar.Value;

            float effectLength = mCurrentEffect.LengthInSeconds;

            start /= 100;
            end /= 100;
            start = CGlobals.DeltaClamp(start);
            end = CGlobals.DeltaClamp(end);
            end = 1 - end;

            mCurrentEffect.Zoom.InitialDelay = start;
            mCurrentEffect.Zoom.EndDelay = end;

            ReDrawZoomDelayTextBoxes();
        }


        private void mZoomEquationComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mCurrentEffect == null || mCurrentEffect.Zoom == null) return;

            mCurrentEffect.Zoom.Equation = GetEquationFromString(this.mZoomEquationComboBox.Text);
        }


        // ********************************************************************************
        //
        // STRETCH 
        //
        // ********************************************************************************

        private void ReDrawStrechDelayTextBoxes()
        {
            if (mCurrentEffect == null || mCurrentEffect.Stretch == null) return;

            float s = (mCurrentEffect.Stretch.InitialDelay * mCurrentEffect.LengthInSeconds) + 0.0499f;
            float e = (mCurrentEffect.Stretch.EndDelay * mCurrentEffect.LengthInSeconds) - 0.0499f;

            mStretchInitialDelayTextBox.Text = System.String.Format("{0:f1}", s);
            mStretchEndDelayTextBox.Text = System.String.Format("{0:f1}", e);
        }

        private void mStretchTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mCurrentEffect == null) return;

            string selected = this.mStretchTypeComboBox.Text;

            if (mCurrentEffect.Stretch != null)
            {
                if (selected == mCurrentEffect.Stretch.ToStretchString())
                {

                    return;
                }
            }

            if (selected == "None")
            {
                mCurrentEffect.Stretch = null;
                this.mStretchStartMultiplier.Enabled = false;
                this.mStretchOffsetMultiplier.Enabled = false;
                this.mStretchEndMultiplier.Enabled = false;
                this.mStretchEquationComboBox.Enabled = false;
                return;
            }

            this.mStretchStartMultiplier.Enabled = true;
            this.mStretchOffsetMultiplier.Enabled = true;
            this.mStretchEndMultiplier.Enabled = true;
            this.mStretchEquationComboBox.Enabled = true;

            if (selected == CVerticalStretch.StretchString())
            {
                mCurrentEffect.Stretch = new CVerticalStretch();
             
            }
            if (selected == CHorizontalStretch.StretchString())
            {
                mCurrentEffect.Stretch = new CHorizontalStretch();
            }

            SetStretchFromControls();
            SetStretchEquationFromControls();
        }

        private void mStretchEquationComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetStretchEquationFromControls();
        }

        private void SetStretchEquationFromControls()
        {
            if (mCurrentEffect == null || mCurrentEffect.Stretch == null) return;

            mCurrentEffect.Stretch.Equation = GetEquationFromString(this.mStretchEquationComboBox.Text);
        }

        private void SetStretchFromControls()
        {
            if (mCurrentEffect == null || mCurrentEffect.Stretch == null) return;

            float sm = mCurrentEffect.Stretch.StartMultiplier;
            if (ConvertAndCheckFloat(mStretchStartMultiplier.Text, ref sm))
            {
                mCurrentEffect.Stretch.StartMultiplier = sm;
            }
            else
            {
                mStretchStartMultiplier.Text = sm.ToString();
            }

            float em = mCurrentEffect.Stretch.EndMultiplier;
            if (ConvertAndCheckFloat(mStretchEndMultiplier.Text, ref em))
            {
                mCurrentEffect.Stretch.EndMultiplier = em;
            }
            else
            {
                mStretchEndMultiplier.Text = em.ToString();
            }
        }

        private void mStretchStartMultiplier_Leave(object sender, EventArgs e)
        {
            SetStretchFromControls();
        }

        private void mStretchEndMultiplier_Leave(object sender, EventArgs e)
        {
            SetStretchFromControls();
        }

        private void mStretchInitialDelayTrackBar_Scroll(object sender, EventArgs e)
        {
            if (mStretchInitialDelayTrackBar.Value >
               mStretchEndDelayTrackBar.Value)
            {
                mStretchInitialDelayTrackBar.Value = mStretchEndDelayTrackBar.Value;
            }

            ReCalcStrechDelayTimes();
        }

        private void mStretchEndDelayTrackBar_Scroll(object sender, EventArgs e)
        {
             if (mStretchEndDelayTrackBar.Value <
               mStretchInitialDelayTrackBar.Value)
            {
                mStretchEndDelayTrackBar.Value = mStretchInitialDelayTrackBar.Value;
            }

            ReCalcStrechDelayTimes();
        }

        private void ReCalcStrechDelayTimes()
        {
            if (mCurrentEffect == null || mCurrentEffect.Stretch == null) return;

            float start = this.mStretchInitialDelayTrackBar.Value;
            float end = mStretchEndDelayTrackBar.Value;

            float effectLength = mCurrentEffect.LengthInSeconds;

            start /= 100;
            end /= 100;
            start = CGlobals.DeltaClamp(start);
            end = CGlobals.DeltaClamp(end);
            end = 1 - end;

            mCurrentEffect.Stretch.InitialDelay = start;
            mCurrentEffect.Stretch.EndDelay = end;

            ReDrawStrechDelayTextBoxes();
        }

        // ********************************************************************************
        //
        // Character delay
        //
        // ********************************************************************************
        private void mCharacterDelayCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mCurrentEffect == null) return;

            mCurrentEffect.HasCharacterTimeDelay = mCharacterDelayCheckBox.Checked;
        }

        private void mCharacterOrderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mCurrentEffect == null) return;

            mCurrentEffect.CharacterOrder = ((CAnimatedDecorationEffect.CharacterDelayOrder)mCharacterOrderComboBox.SelectedIndex);
        }

        private void mCharacterDelayTextBox_Leave(object sender, EventArgs e)
        {
            if (mCurrentEffect == null) return;

            float value = mCurrentEffect.CharacerTimeDelayInSeconds;
            try
            {
                mCurrentEffect.CharacerTimeDelayInSeconds = float.Parse(mCharacterDelayTextBox.Text);
            }
            catch
            {
                mCharacterDelayTextBox.Text = value.ToString();
            }
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }


        // ********************************************************************************
        private void mLengthSetToLengthOfDecorationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mCurrentEffect == null)
            {
                return;
            }

            if (mLengthSetToLengthOfDecorationCheckBox.Checked == true)
            {
                mCurrentEffect.LengthSetToDecorationLength = true;
            }
            else
            {
                mCurrentEffect.LengthSetToDecorationLength = false;
            }
        }

        // ********************************************************************************
        private void mCloneButton_Click(object sender, EventArgs e)
        {
            if (mCurrentEffect == null) return;

            CAnimatedDecorationEffect clonedEffect = mCurrentEffect.XMLClone();
            clonedEffect.TemplateOnlyEffect = true;

            AnimatedDecorationNamer form = new AnimatedDecorationNamer(AnimatedDecorationNamer.AnimatedDecorationNamerType.Rename);
            form.EffectName = clonedEffect.Name;
            DialogResult result = form.ShowDialog();

            if (result == DialogResult.OK)
            {
                clonedEffect.Name = form.EffectName;

                if (CAnimatedDecorationEffectDatabase.GetInstance().EffectExists(clonedEffect.Name) == true)
                {
                    MessageBox.Show("Effect '" + clonedEffect.Name + "' already exists", "Effect already exists", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
        
                CAnimatedDecorationEffectDatabase.GetInstance().AddEffect(clonedEffect);
                RebuildEffectsListComboBox(clonedEffect.Name);
            }
            
        }

        // ********************************************************************************
        private void mRenameButton_Click(object sender, EventArgs e)
        {
            if (mCurrentEffect == null) return;

            string oldname = mCurrentEffect.Name;

            AnimatedDecorationNamer form = new AnimatedDecorationNamer(AnimatedDecorationNamer.AnimatedDecorationNamerType.Rename);
            form.EffectName = mCurrentEffect.Name;
            DialogResult result = form.ShowDialog();

            if (result == DialogResult.OK && oldname != form.EffectName)
            {

                if (CAnimatedDecorationEffectDatabase.GetInstance().EffectExists(form.EffectName) == true)
                {
                    MessageBox.Show("Effect '" + form.EffectName + "' already exists", "Effect already exists", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                mCurrentEffect.Name = form.EffectName;

                // loop through all anmated decorations and possible rename

                List<CDecoration> decorations = CGlobals.mCurrentProject.GetAllDecorations();
                foreach (CDecoration dec in decorations)
                {
                    CAnimatedDecoration animatedDecor = dec as CAnimatedDecoration;
                    if (animatedDecor != null)
                    {
                        if (animatedDecor.MoveInEffectName == oldname)
                        {
                            animatedDecor.MoveInEffectName = mCurrentEffect.Name;
                        }

                        if (animatedDecor.MoveOutEffectName == oldname)
                        {
                            animatedDecor.MoveOutEffectName = mCurrentEffect.Name;
                        }
                    }
                }

                //
                // Loop through all effects to see if UV effect match
                //
                List<CAnimatedDecorationEffect> effects = CAnimatedDecorationEffectDatabase.GetInstance().Effects;
                foreach (CAnimatedDecorationEffect effect in effects)
                {
                    if (effect.UVAnimatedEffectName == oldname)
                    {
                        effect.UVAnimatedEffectName = mCurrentEffect.Name;
                    }
                }

                RebuildEffectsListComboBox(mCurrentEffect.Name);
            }
        }

        // ********************************************************************************
        private void mMotionOutEffectRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (mMotionOutEffectRadio.Checked == true)
            {
                mCurrentEffect = mForDecoration.MoveOutEffect;

                string name = "";
                if (mForDecoration.MoveOutEffect != null)
                {
                    name = mForDecoration.MoveOutEffect.Name;
                }

                RebuildEffectsListComboBox(name);
            }
        }

        private void mMotionInEffectRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (mMotionInEffectRadio.Checked == true)
            {
                mCurrentEffect = mForDecoration.MoveInEffect;

                string name = "";
                if (mForDecoration.MoveInEffect != null)
                {
                    name = mForDecoration.MoveInEffect.Name;
                }

                RebuildEffectsListComboBox(name);
            }
        }

        private void mDoneButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
