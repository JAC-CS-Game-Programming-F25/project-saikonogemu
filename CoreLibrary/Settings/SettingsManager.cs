using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using CoreLibrary;
using CoreLibrary.Graphics;
using CoreLibrary.UI;
using Gum.Forms.Controls;
using Gum.Wireframe;
using Microsoft.Xna.Framework.Audio;
using MonoGameGum;
using MonoGameGum.GueDeriving;

public class SettingsManager
{
    #region Fields
    private readonly string _settingsFile;
    private readonly string _settingsDir;
    [JsonIgnore]
    private AnimatedButton _optionsBackButton;
    [JsonIgnore]
    private Panel _callingPanel;
    [JsonIgnore]
    private AnimatedButton _callingButton;
    #endregion Fields

    #region Properties
    /// <summary>
    /// The graphics settings instance.
    /// </summary>
    public GraphicsSettings Graphics { get; set; } = new GraphicsSettings();

    /// <summary>
    /// The audio settings instance.
    /// </summary>
    public AudioSettings Audio { get; set; } = new AudioSettings();

    /// <summary>
    /// The game settings instance.
    /// </summary>
    public GameplaySettings Gameplay { get; set; } = new GameplaySettings();

    /// <summary>
    /// The UI panel for the settings.
    /// </summary>
    [JsonIgnore]
    public Panel OptionsPanel {get; private set;}
    #endregion

    #region Constructors
    public SettingsManager()
    {
        // Gets cross-platform user data directory.
        string basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        // Sets settings directory.
        _settingsDir = Path.Combine(basePath, "DieTheRollingDiceGame");
        Directory.CreateDirectory(_settingsDir);

        // Sets the file name.
        _settingsFile = Path.Combine(_settingsDir, "settings.json");
    }
    #endregion Constructors

    #region IO Methods
    /// <summary>
    /// Loads the settings from the settings file.
    /// If a settings file does not exits, it creates one and applies default settings.
    /// </summary>
    public void Load()
    {
        // If the file doesn't exist, save the defaults and exit.
        if (!File.Exists(_settingsFile))
        {
            Save();
            return;
        }

        // Gets data from the json.
        string json = File.ReadAllText(_settingsFile);

        // Converts the json into actual data.
        var data = JsonSerializer.Deserialize<SettingsManager>(json);

        // Maps information
        Graphics = data.Graphics;
        Audio = data.Audio;
        Gameplay = data.Gameplay;
    }

    /// <summary>
    /// Saves current settings back to the settings.json file.
    /// </summary>
    public void Save()
    {
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_settingsFile, json);
    }
    #endregion IO Methods

    #region UI Methods
    /// <summary>
    /// Creates a UI Panel that shows all the settings
    /// </summary>
    public void CreateOptionsPanel(TextureAtlas atlas)
    {
        OptionsPanel = new Panel();
        OptionsPanel.Dock(Dock.Fill);
        OptionsPanel.IsVisible = false;
        OptionsPanel.AddToRoot();

        TextRuntime optionsText = new TextRuntime();
        optionsText.X = 10;
        optionsText.Y = 20;
        optionsText.Text = "OPTIONS";
        optionsText.UseCustomFont = true;
        optionsText.FontScale = 1.5f;
        optionsText.CustomFontFile = @"fonts/WhitePeaberry.fnt";
        optionsText.Anchor(Anchor.Top);
        OptionsPanel.AddChild(optionsText);

        OptionsSlider musicSlider = new OptionsSlider(atlas);
        musicSlider.Name = "MusicSlider";
        musicSlider.Text = "MUSIC";
        musicSlider.Anchor(Anchor.Top);
        musicSlider.Visual.Y = 40f;
        musicSlider.Minimum = 0;
        musicSlider.Maximum = 1;
        musicSlider.Value = Core.Audio.SongVolume;
        musicSlider.SmallChange = .1;
        musicSlider.LargeChange = .2;
        musicSlider.ValueChanged += HandleMusicSliderValueChanged;
        musicSlider.ValueChangeCompleted += HandleMusicSliderValueChangeCompleted;
        OptionsPanel.AddChild(musicSlider);

        OptionsSlider sfxSlider = new OptionsSlider(atlas);
        sfxSlider.Name = "SfxSlider";
        sfxSlider.Text = "SFX";
        sfxSlider.Anchor(Anchor.Top);
        sfxSlider.Visual.Y = 103;
        sfxSlider.Minimum = 0;
        sfxSlider.Maximum = 1;
        sfxSlider.Value = Core.Audio.SoundEffectVolume;
        sfxSlider.SmallChange = .1;
        sfxSlider.LargeChange = .2;
        sfxSlider.ValueChanged += HandleSfxSliderChanged;
        sfxSlider.ValueChangeCompleted += HandleSfxSliderChangeCompleted;
        OptionsPanel.AddChild(sfxSlider);

        _optionsBackButton = new AnimatedButton(atlas);
        _optionsBackButton.Text = "BACK";
        _optionsBackButton.Anchor(Anchor.BottomRight);
        _optionsBackButton.X = -28f;
        _optionsBackButton.Y = -10f;
        _optionsBackButton.Click += HandleOptionsButtonBack;
        OptionsPanel.AddChild(_optionsBackButton);
    }

    /// <summary>
    /// Enables the option panel.
    /// </summary>
    public void ActivateOptionsPanel(Panel callingPanel, AnimatedButton callingButton)
    {
        _callingPanel = callingPanel;
        _callingButton = callingButton;

        // Set the options panel to be visible.
        OptionsPanel.IsVisible = true;

        // Give the back button on the options panel focus.
        _optionsBackButton.IsFocused = true;
    }

    private void HandleSfxSliderChanged(object sender, EventArgs args)
    {
        // Intentionally not playing the UI sound effect here so that it is not
        // constantly triggered as the user adjusts the slider's thumb on the
        // track.

        // Get a reference to the sender as a Slider.
        var slider = (Slider)sender;

        // Set the global sound effect volume to the value of the slider.;
        Core.Audio.SoundEffectVolume = (float)slider.Value;
    }

    private void HandleSfxSliderChangeCompleted(object sender, EventArgs e)
    {
        // Play the UI Sound effect so the player can hear the difference in audio.
        Core.Audio.PlaySoundEffect(Core.UISoundEffect);
    }

    private void HandleMusicSliderValueChanged(object sender, EventArgs args)
    {
        // Intentionally not playing the UI sound effect here so that it is not
        // constantly triggered as the user adjusts the slider's thumb on the
        // track.

        // Get a reference to the sender as a Slider.
        var slider = (Slider)sender;

        // Set the global song volume to the value of the slider.
        Core.Audio.SongVolume = (float)slider.Value;
    }

    private void HandleMusicSliderValueChangeCompleted(object sender, EventArgs args)
    {
        // A UI interaction occurred, play the sound effect.
        Core.Audio.PlaySoundEffect(Core.UISoundEffect);
    }

    /// <summary>
    /// Handles input for when the back button is pressed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandleOptionsButtonBack(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect.
        Core.Audio.PlaySoundEffect(Core.UISoundEffect);

        // Set the title panel to be visible.
        _callingPanel.IsVisible = true;

        // Set the options panel to be invisible.
        OptionsPanel.IsVisible = false;

        // Give the options button on the title panel focus since we are coming
        // back from the options screen.
        _callingButton.IsFocused = true;
    }
    #endregion UI Methods
}