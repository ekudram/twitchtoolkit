/*
 * Project: Twitch Toolkit
 * File: CommandEditor.cs
 * 
 * Usage: Manages loading, saving, and backing up of Twitch command definitions to and from JSON files.
 * Is deprecated and will be removed in a future update.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SimpleJSON;
using TwitchToolkit.Utilities;
using Verse;

namespace TwitchToolkit.Commands;

[StaticConstructorOnStartup]
public static class CommandEditor
{
	private static readonly List<Command> commandBackups;

	public static string dataPath;

	public static string editorPath;

	static CommandEditor()
	{
		commandBackups = new List<Command>();
		dataPath = SaveHelper.dataPath;
		editorPath = dataPath + "Editor/Commands/";
		List<Command> allCommands = DefDatabase<Command>.AllDefs.ToList();
		foreach (Command cmd in allCommands)
		{
			Command backup = new Command
			{
				defName = ((Def)cmd).defName,
				command = cmd.command,
				label = ((Def)cmd).label,
				enabled = cmd.enabled,
				shouldBeInSeparateRoom = cmd.shouldBeInSeparateRoom,
				requiresMod = cmd.requiresMod,
				requiresAdmin = cmd.requiresAdmin,
				outputMessage = cmd.outputMessage,
				isCustomMessage = cmd.isCustomMessage
			};
			commandBackups.Add(backup);
		}
		LoadCopies();
	}

	private static void LoadCopies()
	{
		if (!EditorPathExists())
		{
			ToolkitLogger.Error("Path for custom commands does not exist, creating");
			return;
		}
		List<Command> allCommands = DefDatabase<Command>.AllDefs.ToList();
		foreach (Command cmd in allCommands)
		{
			if (CopyExists(cmd))
			{
				LoadCopy(cmd);
			}
		}
		if (ToolkitSettings.CustomCommandDefs == null)
		{
			return;
		}
		foreach (string custom in ToolkitSettings.CustomCommandDefs)
		{
            //ToolkitLogger.Debug("Loading custom command with defName " + custom);
			Command newCustom = new Command
			{
				defName = custom
			};
			if (CopyExists(newCustom))
			{
				LoadCopy(newCustom);
				((Def)newCustom).defName = custom;
				DefDatabase<Command>.Add(newCustom);
			}
		}
	}

	public static bool CopyExists(Command command)
	{
		if (!EditorPathExists())
		{
			throw new DirectoryNotFoundException();
		}
		string filePath = ((Def)command).defName + ".json";
		return File.Exists(editorPath + filePath);
	}

	private static void LoadCopy(Command command)
	{
		string filePath = ((Def)command).defName + ".json";
		try
		{
            var initialCommands = DefDatabase<Command>.AllDefs.ToList();

            List<Command> allCommands = DefDatabase<Command>.AllDefs.ToList();
            ToolkitLogger.Log($"CommandEditor: Found {allCommands.Count} commands in DefDatabase");

            using StreamReader reader = File.OpenText(editorPath + filePath);
			string json = reader.ReadToEnd();
			JSONNode node = JSON.Parse(json);
			if (node["command"] == null)
			{
                ToolkitLogger.Error("Copy of command file is missing critical info, delete file " + editorPath + filePath);
			}
			command.command = node["command"];
			if (node["enabled"] == null)
			{
                ToolkitLogger.Error("Copy of command file is missing critical info, delete file " + editorPath + filePath);
			}
			command.enabled = node["enabled"].AsBool;
			if (node["shouldBeInSeparateRoom"] == null)
			{
                ToolkitLogger.Error("Copy of command file is missing critical info, delete file " + editorPath + filePath);
			}
			command.shouldBeInSeparateRoom = node["shouldBeInSeparateRoom"].AsBool;
			if (node["requiresMod"] == null)
			{
                ToolkitLogger.Error("Copy of command file is missing critical info, delete file " + editorPath + filePath);
			}
			command.requiresMod = node["requiresMod"].AsBool;
			if (node["requiresAdmin"] == null)
			{
                ToolkitLogger.Error("Copy of command file is missing critical info, delete file " + editorPath + filePath);
			}
			command.requiresAdmin = node["requiresAdmin"].AsBool;
			if (node["outputMessage"] == null)
			{
                ToolkitLogger.Error("Copy of command file is missing critical info, delete file " + editorPath + filePath);
			}
			command.outputMessage = node["outputMessage"];
			if (node["isCustomMessage"] == null)
			{
                ToolkitLogger.Error("Copy of command file is missing critical info, delete file " + editorPath + filePath);
			}
			command.isCustomMessage = node["isCustomMessage"].AsBool;
		}
		catch (UnauthorizedAccessException e)
		{
            ToolkitLogger.Error(e.Message);
		}
	}

    private static bool EditorPathExists()
    {
        bool dataPathExists = Directory.Exists(editorPath);
		ToolkitLogger.Debug($"CommandEditor: Editor path '{editorPath}' exists: {dataPathExists}");

        if (!dataPathExists)
        {
            ToolkitLogger.Debug($"CommandEditor: Creating directory '{editorPath}'");
            Directory.CreateDirectory(editorPath);
        }
        return dataPathExists;
    }

    public static void LoadBackups()
	{
		foreach (Command backup in commandBackups)
		{
			LoadBackup(backup);
			SaveCopy(backup);
		}
	}

	public static void LoadBackup(Command backup)
	{
		string defName = ((Def)backup).defName;
		Command inDatabase = DefDatabase<Command>.GetNamed(defName, true);
		inDatabase.command = backup.command;
		inDatabase.enabled = backup.enabled;
	}

	public static void SaveCopy(Command command)
	{
		if (!EditorPathExists())
		{
			throw new DirectoryNotFoundException();
		}
		ToolkitLogger.Debug($"SaveCopy of Commands");
		string filePath = ((Def)command).defName + ".json";
		StringBuilder json = new StringBuilder();
		json.AppendLine("{");
		json.AppendLine("\t\"defName\":\"" + ((Def)command).defName + "\",");
		json.AppendLine("\t\"command\":\"" + command.command + "\",");
		json.AppendLine("\t\"enabled\":\"" + command.enabled + "\",");
		json.AppendLine("\t\"shouldBeInSeparateRoom\":\"" + command.shouldBeInSeparateRoom + "\",");
		json.AppendLine("\t\"requiresMod\":\"" + command.requiresMod + "\",");
		json.AppendLine("\t\"requiresAdmin\":\"" + command.requiresAdmin + "\",");
		json.AppendLine("\t\"outputMessage\":\"" + command.outputMessage + "\",");
		json.AppendLine("\t\"isCustomMessage\":\"" + command.isCustomMessage + "\"");
		json.AppendLine("}");
		using StreamWriter streamWriter = File.CreateText(editorPath + filePath);
		streamWriter.Write(json.ToString());
	}
}
