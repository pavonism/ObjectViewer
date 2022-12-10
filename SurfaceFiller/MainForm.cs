using SketcherControl;
using SketcherControl.Filling;
using SurfaceFiller.Components;
using SurfaceFiller.Samples;
using System.Drawing.Imaging;

namespace SurfaceFiller
{
    public partial class MainForm : Form
    {
        private TableLayoutPanel mainTableLayout = new();
        private Toolbar toolbar = new() { Width = FormConstants.ToolbarWidth };
        private Sketcher sketcher = new();
        private ComboPicker<BasicSample> objectCombo;
        private ComboPickerWithImage<Sample> objectSurfaceCombo;
        private ComboPickerWithImage<Sample> normalMapCombo;
        private ColorSample colorSample;

        public MainForm()
        {
            InitializeToolbar();
            ArrangeComponents();
            InitializeForm();
            LoadTextureSamples();
            LoadNormalMapSamples();
            LoadObjectSamples();
        }

        #region Initialization
        private void InitializeToolbar()
        {
            this.toolbar.AddLabel(Resources.ProgramTitle);
            this.toolbar.AddDivider();
            this.objectCombo = this.toolbar.AddComboPicker<BasicSample>(ObjectPickedHandler);
            this.toolbar.AddButton(OpenFileHandler, Glyphs.File, Hints.OpenOBJ);
            this.toolbar.AddTool(FillHandler, Glyphs.Bucket, Hints.Fill);
            this.toolbar.AddSlider(ThreadsSlidrerHandler, Labels.ThreadSlider, Defaults.ThreadsCount);
            this.toolbar.AddDivider();
            this.toolbar.AddOption(ShowLinesHandler, Labels.ShowLinesOption, Hints.ShowLines);
            this.toolbar.CreateNewRadioBox();
            this.toolbar.AddRadioOption(ColorInterpolationOptionHandler, Labels.ColorInterpolationOption, Hints.ColorInterpolation, true);
            this.toolbar.AddRadioOption(VectorInterpolationOptionHandler, Labels.VectorsInterpolationOption, Hints.VectorInterpolation);
            this.toolbar.AddDivider();
            this.toolbar.AddLabel(Labels.ModelParameters);
            this.toolbar.AddFractSlider(KDParameterHandler, Labels.KDParameter, Defaults.KDParameter);
            this.toolbar.AddFractSlider(KSParameterHandler, Labels.KSParameter, Defaults.KSParameter);
            this.toolbar.AddSlider(MParameterHandler, Labels.MParameter, Defaults.MParameter);
            this.toolbar.AddDivider();
            this.toolbar.AddLabel(Labels.LightSection);
            this.toolbar.AddPlayPouse(SunHandler, true);
            this.toolbar.AddProcessButton(RewindHandler, Glyphs.Rewind);
            this.toolbar.AddProcessButton(MoveForwardHandler, Glyphs.Forward);
            this.toolbar.AddButton(LightColorButtonHandler, Glyphs.Palette, Hints.ChangeLightColor);
            this.toolbar.AddTool(ShowTrackHandler, Glyphs.Spiral, Hints.ShowTrack);
            this.toolbar.AddButton(ResetPositionButtonHandler, Glyphs.Reset, Hints.ResetPosition);
            this.toolbar.AddSlider(SunSpeedHanlder, Labels.Speed, Defaults.AnimationSpeed);
            this.toolbar.AddSlider(SunZLocationHandler, Labels.ZLocation, Defaults.LightLocationZ);
            this.toolbar.AddDivider();
            this.toolbar.AddLabel(Labels.ObjectSurface);
            this.objectSurfaceCombo = this.toolbar.AddComboImagePicker<Sample>(TexturePickedHandler);
            this.toolbar.AddButton(ObjectColorButtonHandler, Glyphs.Palette, Hints.ChangeObjectColor);
            this.toolbar.AddButton(LoadTextureHandlar, Glyphs.File, Hints.LoadObjectPattern);
            this.normalMapCombo = this.toolbar.AddComboImagePicker<Sample>(NormalMapPickedHandler);
            this.toolbar.AddButton(VectorMapHandler, Glyphs.File, Hints.LoadNormalMap);
        }

        private void InitializeForm()
        {
            this.Text = Resources.ProgramTitle;
            this.MinimumSize = new Size(FormConstants.MinimumWindowSizeX, FormConstants.MinimumWindowSizeY);
            this.Size = new Size(FormConstants.InitialWindowSizeX, FormConstants.InitialWindowSizeY);
        }

        private void ArrangeComponents()
        {
            this.mainTableLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            this.mainTableLayout.ColumnCount = FormConstants.MainFormColumnCount;

            this.mainTableLayout.Controls.Add(this.sketcher, 1, 0);
            this.mainTableLayout.Controls.Add(this.toolbar, 0, 0);
            this.mainTableLayout.Dock = DockStyle.Fill;
            this.Controls.Add(mainTableLayout);
        }
        #endregion

        #region Handlers 
        private void VectorInterpolationOptionHandler(object? sender, EventArgs e)
        {
            this.sketcher.ColorPicker.InterpolationMode = Interpolation.NormalVector;
        }

        private void ColorInterpolationOptionHandler(object? sender, EventArgs e)
        {
            this.sketcher.ColorPicker.InterpolationMode = Interpolation.Color;
        }

        private void ObjectPickedHandler(BasicSample newValue)
        {
            if (newValue is ObjectSample objectSample)
            {
                this.sketcher.LoadObjectFromFile(objectSample.Path);
            }
        }

        private void NormalMapPickedHandler(Sample newValue)
        {
            if (newValue is PictureSample pictureSample)
            {
                this.sketcher.ColorPicker.NormalMap = pictureSample.Image;
            }
        }

        private void TexturePickedHandler(Sample newValue)
        {
            if(newValue is PictureSample pictureSample)
            {
                this.sketcher.ColorPicker.Pattern = pictureSample.Image;
            }
            else if(newValue is ColorSample colorSample)
            {
                this.sketcher.ColorPicker.Pattern = null;
                this.sketcher.ColorPicker.TargetColor = colorSample.Color;
            }
        }

        private void VectorMapHandler(object? sender, EventArgs e)
        {
            var normalMapSample = OpenLoadImageDialog();

            if (normalMapSample != null)
            {
                this.sketcher.ColorPicker.NormalMap = normalMapSample.Image;
                this.normalMapCombo.AddAndSelect(normalMapSample);
            }
        }

        private void MoveForwardHandler()
        {
            this.sketcher.LightSource.MoveLight();
            this.sketcher.Refresh();
        }

        private void RewindHandler()
        {
            this.sketcher.LightSource.MoveLight(true);
            this.sketcher.Refresh();
        }

        private void LoadTextureHandlar(object? sender, EventArgs e)
        {
            var textureSample = OpenLoadImageDialog();

            if (textureSample != null)
            {
                this.sketcher.ColorPicker.Pattern = textureSample.Image;
                this.objectSurfaceCombo.AddAndSelect(textureSample);
            }
        }

        private void NormalVectorsHandler(object? sender, EventArgs e)
        {
            this.sketcher.ColorPicker.InterpolationMode = this.sketcher.ColorPicker.InterpolationMode == Interpolation.Color ? Interpolation.NormalVector : Interpolation.Color;
        }

        private void ThreadsSlidrerHandler(float value)
        {
            this.sketcher.RenderThreads = (int)(value * 100);
        }

        private void ShowTrackHandler(bool obj)
        {
            this.sketcher.LightSource.ShowTrack = !this.sketcher.LightSource.ShowTrack;
        }

        private void MParameterHandler(float newValue)
        {
            this.sketcher.ColorPicker.M = (int)(100 * newValue);
        }

        private void KSParameterHandler(float newValue)
        {
            this.sketcher.ColorPicker.KS = newValue;
        }

        private void KDParameterHandler(float newValue)
        {
            this.sketcher.ColorPicker.KD = newValue;
        }

        private void SunZLocationHandler(float newValue)
        {
            this.sketcher.LightSource.LightLocationZ = newValue;
        }

        private void SunSpeedHanlder(float newValue)
        {
            this.sketcher.LightSource.LightSpeed = newValue;
        }

        private void SunHandler(bool newValue)
        {
            this.sketcher.LightSource.LightAnimation = !this.sketcher.LightSource.LightAnimation;
        }

        private void ShowLinesHandler(object? sender, EventArgs e)
        {
            this.sketcher.ShowLines = !this.sketcher.ShowLines;
            this.sketcher.Refresh();
        }

        private void FillHandler(bool obj)
        {
            this.sketcher.Fill = !this.sketcher.Fill;
            this.sketcher.Refresh();
        }

        private void OpenFileHandler(object? sender, EventArgs e)
        {
            var fileName = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Object files (*.obj)|*.obj|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = openFileDialog.FileName;
                }
            }

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                this.sketcher.LoadObjectFromFile(fileName);
                this.objectCombo.AddAndSelect(SampleGenerator.CreateObjectSample(fileName));
            }
        }

        private void ResetPositionButtonHandler(object? sender, EventArgs e)
        {
            this.sketcher.LightSource.Reset();
        }

        private void LightColorButtonHandler(object? sender, EventArgs e)
        {

            ColorDialog MyDialog = new ColorDialog();
            MyDialog.AllowFullOpen = true;
            MyDialog.ShowHelp = true;
            MyDialog.Color = this.sketcher.LightSource.LightSourceColor;

            // Update the text box color if the user clicks OK 
            if (MyDialog.ShowDialog() == DialogResult.OK)
                this.sketcher.LightSource.LightSourceColor = MyDialog.Color;
        }

        private void ObjectColorButtonHandler(object? sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            MyDialog.AllowFullOpen = true;
            MyDialog.ShowHelp = true;
            MyDialog.Color = this.sketcher.LightSource.LightSourceColor;

            // Update the text box color if the user clicks OK 
            if (MyDialog.ShowDialog() == DialogResult.OK)
            {
                this.sketcher.ColorPicker.TargetColor = MyDialog.Color;
                this.colorSample.Color = MyDialog.Color;
                this.objectSurfaceCombo.Select(colorSample);
                this.objectSurfaceCombo.Refresh();
            }
        }
        #endregion

        #region Loading Samples
        private void LoadTextureSamples()
        {
            this.objectSurfaceCombo.AddOptions(SampleGenerator.GetTextures(Resources.TextureAssets, out this.colorSample));
            this.objectSurfaceCombo.DefaultValue = this.colorSample;
        }

        private void LoadNormalMapSamples()
        {
            var samples = SampleGenerator.GetNormalMaps(Resources.NormalMapsAssets);
            this.normalMapCombo.AddOptions(samples);
            this.normalMapCombo.DefaultValue = samples.FirstOrDefault();
        }

        private void LoadObjectSamples()
        {
            var samples = SampleGenerator.GetObjectSamples(Resources.ObjectAssets);
            this.objectCombo.AddOptions(samples);
            this.objectCombo.DefaultValue = samples.FirstOrDefault();
        }

        private PictureSample? OpenLoadImageDialog()
        {
            var fileContent = string.Empty;
            string filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Object files (*.obj)|*.obj|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                var codecs = ImageCodecInfo.GetImageEncoders();
                var codecFilter = "Image Files|";
                foreach (var codec in codecs)
                {
                    codecFilter += codec.FilenameExtension + ";";
                }
                openFileDialog.Filter = codecFilter;


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                }
            }

            if (!string.IsNullOrWhiteSpace(filePath))
                return SampleGenerator.GetSample(filePath);

            return null;
        }
        #endregion
    }
}