using System;
using System.Drawing;
using System.Windows.Forms;
using Satori.Client;

namespace Satori.Windows;

internal static class Program
{
	[STAThread]
	private static void Main(string[] args)
	{
		ApplicationConfiguration.Initialize();

		bool forceLauncher = Array.Exists(args, arg => string.Equals(arg, "--launcher", StringComparison.OrdinalIgnoreCase));
		if (!forceLauncher && LauncherPreferences.ShouldSkipLauncher())
		{
			GameRunner.Run(GameLaunchOptions.Default);
			return;
		}

		Application.Run(new LauncherForm());
	}
}

internal static class GameRunner
{
	public static void Run(GameLaunchOptions launchOptions)
	{
		using SatoriGame game = new SatoriGame(launchOptions);
		game.Run();
	}
}

internal sealed class LauncherForm : Form
{
	private readonly CheckBox _skipLauncherCheckBox;

	public LauncherForm()
	{
		Text = "Satori";
		ClientSize = new Size(360, 220);
		FormBorderStyle = FormBorderStyle.FixedDialog;
		MaximizeBox = false;
		StartPosition = FormStartPosition.CenterScreen;
		BackColor = Color.FromArgb(18, 22, 28);
		ForeColor = Color.FromArgb(230, 225, 210);

		var title = new Label
		{
			Text = "Satori",
			Font = new Font(Font.FontFamily, 16f, FontStyle.Bold),
			AutoSize = false,
			TextAlign = ContentAlignment.MiddleCenter,
			Dock = DockStyle.Top,
			Height = 48
		};

		var hint = new Label
		{
			Text = "Паломничество, медитация и путь к просветлению.",
			AutoSize = false,
			TextAlign = ContentAlignment.MiddleCenter,
			Dock = DockStyle.Fill,
			Padding = new Padding(16, 8, 16, 8)
		};

		_skipLauncherCheckBox = new CheckBox
		{
			Text = "Запускать игру сразу",
			AutoSize = true,
			Dock = DockStyle.Bottom,
			Padding = new Padding(16, 0, 16, 8),
			ForeColor = ForeColor,
			Checked = LauncherPreferences.ShouldSkipLauncher()
		};

		var buttonPanel = new FlowLayoutPanel
		{
			Dock = DockStyle.Bottom,
			Height = 48,
			Padding = new Padding(16, 0, 16, 12),
			FlowDirection = FlowDirection.LeftToRight,
			WrapContents = false
		};

		var settingsButton = CreateActionButton("Настройки");
		settingsButton.Click += (_, _) => LaunchGame(GameLaunchOptions.OpenSettings);

		var playButton = CreateActionButton("Играть");
		playButton.Click += (_, _) => LaunchGame(GameLaunchOptions.Default);

		buttonPanel.Controls.Add(playButton);
		buttonPanel.Controls.Add(settingsButton);

		Controls.Add(hint);
		Controls.Add(buttonPanel);
		Controls.Add(_skipLauncherCheckBox);
		Controls.Add(title);
	}

	private Button CreateActionButton(string text)
	{
		return new Button
		{
			Text = text,
			Width = 150,
			Height = 32,
			FlatStyle = FlatStyle.Flat,
			BackColor = Color.FromArgb(72, 88, 104),
			ForeColor = Color.White,
			Margin = new Padding(0, 0, 8, 0)
		};
	}

	private void LaunchGame(GameLaunchOptions launchOptions)
	{
		LauncherPreferences.SetSkipLauncher(_skipLauncherCheckBox.Checked);
		Hide();
		try
		{
			GameRunner.Run(launchOptions);
		}
		finally
		{
			Enabled = true;
		}

		if (_skipLauncherCheckBox.Checked)
		{
			Close();
			return;
		}

		Show();
	}
}
