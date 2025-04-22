using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using SkillManager;
using ItemManager;
using JetBrains.Annotations;
using LocalizationManager;
using PieceManager;
using ServerSync;
using UnityEngine;

namespace TrowelMod
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class TrowelModPlugin : BaseUnityPlugin
    {
        internal const string ModName = "TrowelMod";
        internal const string ModVersion = "1.0.0";
        internal const string Author = "Devin";
        private const string ModGUID = Author + "." + ModName;
        private static string ConfigFileName = ModGUID + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;

        internal static string ConnectionError = "";

        private readonly Harmony _harmony = new(ModGUID);

        public static readonly ManualLogSource TrowelModLogger =
            BepInEx.Logging.Logger.CreateLogSource(ModName);

        private static readonly ConfigSync ConfigSync = new(ModGUID)
            { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };

        public enum Toggle
        {
            On = 1,
            Off = 0
        }

        internal static Skill FengShui;
        public void Awake()
        {
            // Uncomment the line below to use the LocalizationManager for localizing your mod.
            // Localizer.Load(); // Use this to initialize the LocalizationManager (for more information on LocalizationManager, see the LocalizationManager documentation https://github.com/blaxxun-boop/LocalizationManager#example-project).

            _serverConfigLocked = config("1 - General", "Lock Configuration", Toggle.On, "If on, the configuration is locked and can be changed by server admins only.");
            _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);

            Skill FengShui = new("Feng Shui", "feng.png");
            FengShui.Description.English("Increases comfort gained from comfort items");
            FengShui.Configurable = true;

            Item trowelTool = new("gtrowelbundle", "item_gardentrowel");    
            trowelTool.Name.English("Gardening Trowel");
            trowelTool.Description.English("A tool for gardening projects.");
            trowelTool.RequiredItems.Add("Tin", 5);
            trowelTool.RequiredItems.Add("FineWood", 5);
            trowelTool.Crafting.Add(ItemManager.CraftingTable.Forge, 1);
            trowelTool.Trade.RequiredGlobalKey = "defeated_eikthyr";
            trowelTool.Snapshot();

            // Item fertilizer = new("gtrowelbundle", "item_fertilizer");
            // fertilizer.Name.English("Fertilizer");
            // fertilizer.Description.English("Speed up plant growth!");
            // fertilizer["Recipe1"].Crafting.Add(ItemManager.CraftingTable.Workbench, 2);
            // fertilizer["Recipe1"].RequiredItems.Add("Pukeberries", 3);
            // fertilizer["Recipe1"].RequiredItems.Add("Entrails", 1);
            // fertilizer["Recipe2"].Crafting.Add(ItemManager.CraftingTable.Workbench, 2);
            // fertilizer["Recipe2"].RequiredItems.Add("RawMeat", 1);
            // fertilizer["Recipe2"].RequiredItems.Add("Entrails", 1);
            // fertilizer.Snapshot();

            // Globally turn off configuration options for your pieces, omit if you don't want to do this.
            BuildPiece.ConfigurationEnabled = false;

            BuildPiece fbPiece = new("flowerbedbundle", "piece_flower_bed");
            fbPiece.Name.English("1x1 Flowerbed");
            fbPiece.Description.English("Plant Stuff");
            fbPiece.RequiredItems.Add("FineWood", 20, true);
            fbPiece.RequiredItems.Add("Stone", 15, true);
            fbPiece.Category.Set(PieceManager.BuildPieceCategory.Furniture);
            fbPiece.Crafting.Set(PieceManager.CraftingTable.Workbench);
            fbPiece.Snapshot();
            fbPiece.Prefab.AddComponent<IsFlowerbed>();

            BuildPiece fb3xPiece = new BuildPiece("flowerbedbundle", "piece_3xflower_bed");
            fb3xPiece.Name.English("3x1 Flowerbed");
            fb3xPiece.Description.English("Plant More Stuff");
            fb3xPiece.RequiredItems.Add("FineWood", 40, true);
            fb3xPiece.RequiredItems.Add("Stone", 50, true);
            fb3xPiece.Category.Set(PieceManager.BuildPieceCategory.Furniture);
            fb3xPiece.Crafting.Set(PieceManager.CraftingTable.Workbench);
            fb3xPiece.Snapshot();
            fb3xPiece.Prefab.AddComponent<IsFlowerbed>();

            BuildPiece fb5xPiece = new BuildPiece("flowerbedbundle", "piece_5xflower_bed");
            fb5xPiece.Name.English("5x1 Flowerbed");
            fb5xPiece.Description.English("Plant More Stuff");
            fb5xPiece.RequiredItems.Add("FineWood", 50, true);
            fb5xPiece.RequiredItems.Add("Stone", 60, true);
            fb5xPiece.Category.Set(PieceManager.BuildPieceCategory.Furniture);
            fb5xPiece.Crafting.Set(PieceManager.CraftingTable.Workbench);
            fb5xPiece.Snapshot();
            fb5xPiece.Prefab.AddComponent<IsFlowerbed>();

            BuildPiece blueFB = new BuildPiece("flowerbedbundle", "piece_blue_flowerbed");
            blueFB.Name.English("Blue Flowerbed");
            blueFB.Description.English("A more colorful option to plant!");
            blueFB.RequiredItems.Add("FineWood", 25, true);
            blueFB.RequiredItems.Add("Stone", 20, true);
            blueFB.RequiredItems.Add("Blueberries", 5, true);
            blueFB.Category.Set(PieceManager.BuildPieceCategory.Furniture);
            blueFB.Crafting.Set(PieceManager.CraftingTable.Workbench);
            blueFB.Snapshot();
            blueFB.Prefab.AddComponent<IsFlowerbed>();

            BuildPiece greenFB = new BuildPiece("flowerbedbundle", "piece_green_flowerbed");
            greenFB.Name.English("Green Flowerbed");
            greenFB.Description.English("A more colorful option to plant!");
            greenFB.RequiredItems.Add("FineWood", 25, true);
            greenFB.RequiredItems.Add("Stone", 20, true);
            greenFB.RequiredItems.Add("AncientSeed", 3, true);
            greenFB.Category.Set(PieceManager.BuildPieceCategory.Furniture);
            greenFB.Crafting.Set(PieceManager.CraftingTable.Workbench);
            greenFB.Snapshot();
            greenFB.Prefab.AddComponent<IsFlowerbed>();

            BuildPiece purpleFB = new BuildPiece("flowerbedbundle", "piece_purple_flowerbed");
            purpleFB.Name.English("Purple Flowerbed");
            purpleFB.Description.English("A more colorful option to plant!");
            purpleFB.RequiredItems.Add("FineWood", 25, true);
            purpleFB.RequiredItems.Add("Stone", 20, true);
            purpleFB.RequiredItems.Add("Blueberries", 2, true);
            purpleFB.RequiredItems.Add("Raspberry", 2, true);
            purpleFB.Category.Set(PieceManager.BuildPieceCategory.Furniture);
            purpleFB.Crafting.Set(PieceManager.CraftingTable.Workbench);
            purpleFB.Snapshot();
            purpleFB.Prefab.AddComponent<IsFlowerbed>();

            BuildPiece nsaplingTurnip = new BuildPiece("flowerbedbundle", "N_sapling_turnip");
            nsaplingTurnip.Name.English("Turnip");
            nsaplingTurnip.Description.English("Yummy Turnip");
            nsaplingTurnip.RequiredItems.Add("TurnipSeeds", 1, false);
            nsaplingTurnip.Tool.Add("item_gardentrowel");
            MaterialReplacer.RegisterGameObjectForShaderSwap(nsaplingTurnip.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            nsaplingTurnip.Prefab.AddComponent<IgnoreGrowSpace>();
            nsaplingTurnip.Prefab.AddComponent<RequiresFlowerbed>();
            GameObject pickTurnipPrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "N_Pickable_Turnip");
            MaterialReplacer.RegisterGameObjectForShaderSwap(pickTurnipPrefab, MaterialReplacer.ShaderType.UseUnityShader);

            BuildPiece nsaplingTurnipSeed = new BuildPiece("flowerbedbundle", "N_sapling_seedturnip");
            nsaplingTurnipSeed.Name.English("Seed Turnip");
            nsaplingTurnipSeed.Description.English("Gotta plant Turnip to get more Turnip");
            nsaplingTurnipSeed.RequiredItems.Add("Turnip", 1, false);
            nsaplingTurnipSeed.Tool.Add("item_gardentrowel");
            MaterialReplacer.RegisterGameObjectForShaderSwap(nsaplingTurnipSeed.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            nsaplingTurnipSeed.Prefab.AddComponent<IgnoreGrowSpace>();
            nsaplingTurnipSeed.Prefab.AddComponent<RequiresFlowerbed>();
            GameObject pickTurnipSeedPrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "N_Pickable_SeedTurnip");
            MaterialReplacer.RegisterGameObjectForShaderSwap(pickTurnipSeedPrefab, MaterialReplacer.ShaderType.UseUnityShader);

            BuildPiece nsaplingOnion = new BuildPiece("flowerbedbundle", "N_sapling_onion");
            nsaplingOnion.Name.English("Onion");
            nsaplingOnion.Description.English("Spicy White Orb");
            nsaplingOnion.RequiredItems.Add("OnionSeeds", 1, false);
            nsaplingOnion.Tool.Add("item_gardentrowel");
            MaterialReplacer.RegisterGameObjectForShaderSwap(nsaplingOnion.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            nsaplingOnion.Prefab.AddComponent<IgnoreGrowSpace>();
            nsaplingOnion.Prefab.AddComponent<RequiresFlowerbed>();
            GameObject pickOnionPrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "N_Pickable_Onion");
            MaterialReplacer.RegisterGameObjectForShaderSwap(pickOnionPrefab, MaterialReplacer.ShaderType.UseUnityShader);

            BuildPiece nsaplingOnionSeed = new BuildPiece("flowerbedbundle", "N_sapling_seedonion");
            nsaplingOnionSeed.Name.English("Seed Onion");
            nsaplingOnionSeed.Description.English("Grows cue balls");
            nsaplingOnionSeed.RequiredItems.Add("Onion", 1, false);
            nsaplingOnionSeed.Tool.Add("item_gardentrowel");
            MaterialReplacer.RegisterGameObjectForShaderSwap(nsaplingOnionSeed.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            nsaplingOnionSeed.Prefab.AddComponent<IgnoreGrowSpace>();
            nsaplingOnionSeed.Prefab.AddComponent<RequiresFlowerbed>();
            GameObject pickOnionSeedPrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "N_Pickable_SeedOnion");
            MaterialReplacer.RegisterGameObjectForShaderSwap(pickOnionSeedPrefab, MaterialReplacer.ShaderType.UseUnityShader);


            BuildPiece nsaplingCarrot = new BuildPiece("flowerbedbundle", "N_sapling_carrot");
            nsaplingCarrot.Name.English("Carrot");
            nsaplingCarrot.Description.English("Good for the eyes");
            nsaplingCarrot.RequiredItems.Add("CarrotSeeds", 1, false);
            nsaplingCarrot.Tool.Add("item_gardentrowel");
            MaterialReplacer.RegisterGameObjectForShaderSwap(nsaplingCarrot.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            nsaplingCarrot.Prefab.AddComponent<IgnoreGrowSpace>();
            nsaplingCarrot.Prefab.AddComponent<RequiresFlowerbed>();
            GameObject pickCarrotPrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "N_Pickable_Carrot");
            MaterialReplacer.RegisterGameObjectForShaderSwap(pickCarrotPrefab, MaterialReplacer.ShaderType.UseUnityShader);


            BuildPiece nsaplingCarrotSeed = new BuildPiece("flowerbedbundle", "N_sapling_seedcarrot");
            nsaplingCarrotSeed.Name.English("Seed Carrot");
            nsaplingCarrotSeed.Description.English("Ehhh, what's up doc?");
            nsaplingCarrotSeed.RequiredItems.Add("Carrot_0", 1, false);
            nsaplingCarrotSeed.Tool.Add("item_gardentrowel");
            MaterialReplacer.RegisterGameObjectForShaderSwap(nsaplingCarrotSeed.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            nsaplingCarrotSeed.Prefab.AddComponent<IgnoreGrowSpace>();
            nsaplingCarrotSeed.Prefab.AddComponent<RequiresFlowerbed>();
            GameObject pickCarrotSeedPrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "N_Pickable_SeedCarrot");
            MaterialReplacer.RegisterGameObjectForShaderSwap(pickCarrotSeedPrefab, MaterialReplacer.ShaderType.UseUnityShader);

            BuildPiece nsaplingBarley = new BuildPiece("flowerbedbundle", "N_sapling_barley");
            nsaplingBarley.Name.English("Barley");
            nsaplingBarley.Description.English("There's nothing funny about barley.");
            nsaplingBarley.RequiredItems.Add("Barley_0", 1, false);
            nsaplingBarley.Tool.Add("item_gardentrowel");
            MaterialReplacer.RegisterGameObjectForShaderSwap(nsaplingBarley.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            nsaplingBarley.Prefab.AddComponent<IgnoreGrowSpace>();
            nsaplingBarley.Prefab.AddComponent<RequiresFlowerbed>();
            GameObject pickBarleyPrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "N_Pickable_Barley");
            MaterialReplacer.RegisterGameObjectForShaderSwap(pickBarleyPrefab, MaterialReplacer.ShaderType.UseUnityShader);
            GameObject breakBarleyPrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "vfx_barley_destroyed1");
            MaterialReplacer.RegisterGameObjectForShaderSwap(breakBarleyPrefab, MaterialReplacer.ShaderType.UseUnityShader);

            BuildPiece nsaplingFlax = new BuildPiece("flowerbedbundle", "N_sapling_flax");
            nsaplingFlax.Name.English("Flax");
            nsaplingFlax.Description.English("Flax no printer amirite?");
            nsaplingFlax.RequiredItems.Add("Flax", 1, false);
            nsaplingFlax.Tool.Add("item_gardentrowel");
            MaterialReplacer.RegisterGameObjectForShaderSwap(nsaplingFlax.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            nsaplingFlax.Prefab.AddComponent<IgnoreGrowSpace>();
            nsaplingFlax.Prefab.AddComponent<RequiresFlowerbed>();
            GameObject pickFlaxPrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "N_Pickable_Flax");
            MaterialReplacer.RegisterGameObjectForShaderSwap(pickFlaxPrefab, MaterialReplacer.ShaderType.UseUnityShader);

            BuildPiece nsaplingJotun = new BuildPiece("flowerbedbundle", "N_sapling_jotunpuffs");
            nsaplingJotun.Name.English("Jotun Puffs");
            nsaplingJotun.Description.English("This is a mushroom I guess");
            nsaplingJotun.RequiredItems.Add("MushroomJotunPuffs", 1, false);
            nsaplingJotun.Tool.Add("item_gardentrowel");
            MaterialReplacer.RegisterGameObjectForShaderSwap(nsaplingJotun.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            nsaplingJotun.Prefab.AddComponent<IgnoreGrowSpace>();
            nsaplingJotun.Prefab.AddComponent<RequiresFlowerbed>();
            GameObject pickJotunPrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "N_Pickable_Mushroom_JotunPuffs");
            MaterialReplacer.RegisterGameObjectForShaderSwap(pickJotunPrefab, MaterialReplacer.ShaderType.UseUnityShader);

            BuildPiece nsaplingMagecap = new BuildPiece("flowerbedbundle", "N_sapling_magecap");
            nsaplingMagecap.Name.English("Magecap");
            nsaplingMagecap.Description.English("This is also a mushroom it seems");
            nsaplingMagecap.RequiredItems.Add("MushroomMagecap", 1, false);
            nsaplingMagecap.Tool.Add("item_gardentrowel");
            MaterialReplacer.RegisterGameObjectForShaderSwap(nsaplingMagecap.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            nsaplingMagecap.Prefab.AddComponent<IgnoreGrowSpace>();
            nsaplingMagecap.Prefab.AddComponent<RequiresFlowerbed>();
            GameObject pickMageCPrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "N_Pickable_Mushroom_Magecap");
            MaterialReplacer.RegisterGameObjectForShaderSwap(pickMageCPrefab, MaterialReplacer.ShaderType.UseUnityShader);

            GameObject woodPoleVFXPrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "vfx_Place_wood_pole1");
            MaterialReplacer.RegisterGameObjectForShaderSwap(woodPoleVFXPrefab, MaterialReplacer.ShaderType.UseUnityShader);
            GameObject smallItemVFXPrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "vfx_Place_smallitem1");
            MaterialReplacer.RegisterGameObjectForShaderSwap(smallItemVFXPrefab, MaterialReplacer.ShaderType.UseUnityShader);
            GameObject treeVFXPrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "vfx_firetreecut1");
            MaterialReplacer.RegisterGameObjectForShaderSwap(treeVFXPrefab, MaterialReplacer.ShaderType.UseUnityShader);
            GameObject plantVFXPrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "vfx_turnip_grow1");
            MaterialReplacer.RegisterGameObjectForShaderSwap(plantVFXPrefab, MaterialReplacer.ShaderType.UseUnityShader);
            GameObject sawDustVFXPrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "vfx_SawDust1");
            MaterialReplacer.RegisterGameObjectForShaderSwap(sawDustVFXPrefab, MaterialReplacer.ShaderType.UseUnityShader);

            BuildPiece nfirSapling = new BuildPiece("flowerbedbundle", "N_FirTree_Sapling");
            nfirSapling.Name.English("Fir Sapling");
            nfirSapling.Description.English("This tree is fur?!");
            nfirSapling.RequiredItems.Add("FirCone", 1, false);
            nfirSapling.Tool.Add("item_gardentrowel");
            MaterialReplacer.RegisterGameObjectForShaderSwap(nfirSapling.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            nfirSapling.Prefab.AddComponent<IgnoreGrowSpace>();
            nfirSapling.Prefab.AddComponent<RequiresFlowerbed>();
            nfirSapling.Prefab.AddComponent<SaplingTracker>();
            GameObject grownFirPrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "N_FirTree");
            MaterialReplacer.RegisterGameObjectForShaderSwap(grownFirPrefab, MaterialReplacer.ShaderType.UseUnityShader);

            BuildPiece npineSapling = new BuildPiece("flowerbedbundle", "N_PineTree_Sapling");
            npineSapling.Name.English("Pine Sapling");
            npineSapling.Description.English("Plant a fine lookin Pine");
            npineSapling.RequiredItems.Add("PineSeed", 1, false);
            npineSapling.Tool.Add("item_gardentrowel");
            MaterialReplacer.RegisterGameObjectForShaderSwap(npineSapling.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            npineSapling.Prefab.AddComponent<IgnoreGrowSpace>();
            npineSapling.Prefab.AddComponent<RequiresFlowerbed>();
            npineSapling.Prefab.AddComponent<SaplingTracker>();
            GameObject grownPinePrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "N_Pinetree_01");
            MaterialReplacer.RegisterGameObjectForShaderSwap(grownPinePrefab, MaterialReplacer.ShaderType.UseUnityShader);

            BuildPiece nbeechSapling = new BuildPiece("flowerbedbundle", "N_Beech_Sapling");
            nbeechSapling.Name.English("Beech Sapling");
            nbeechSapling.Description.English("Grows into a Beech Tree!");
            nbeechSapling.RequiredItems.Add("BeechSeeds", 1, false);
            nbeechSapling.Tool.Add("item_gardentrowel");
            MaterialReplacer.RegisterGameObjectForShaderSwap(nbeechSapling.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            nbeechSapling.Prefab.AddComponent<IgnoreGrowSpace>();
            nbeechSapling.Prefab.AddComponent<RequiresFlowerbed>();
            nbeechSapling.Prefab.AddComponent<SaplingTracker>();
            GameObject beechTreePrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "N_Beech1");
            MaterialReplacer.RegisterGameObjectForShaderSwap(beechTreePrefab, MaterialReplacer.ShaderType.UseUnityShader);
            GameObject beechVFXPrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "vfx_beech_cut1");
            MaterialReplacer.RegisterGameObjectForShaderSwap(beechVFXPrefab, MaterialReplacer.ShaderType.UseUnityShader);

            BuildPiece nbirchSapling = new BuildPiece("flowerbedbundle", "N_Birch_Sapling");
            nbirchSapling.Name.English("Birch Sapling");
            nbirchSapling.Description.English("This tree is fur?!");
            nbirchSapling.RequiredItems.Add("BirchSeeds", 1, false);
            nbirchSapling.Tool.Add("item_gardentrowel");
            MaterialReplacer.RegisterGameObjectForShaderSwap(nbirchSapling.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            nbirchSapling.Prefab.AddComponent<IgnoreGrowSpace>();
            nbirchSapling.Prefab.AddComponent<RequiresFlowerbed>();
            nbirchSapling.Prefab.AddComponent<SaplingTracker>();
            GameObject grownBirch1Prefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "N_Birch1");
            MaterialReplacer.RegisterGameObjectForShaderSwap(grownBirch1Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            GameObject grownBirch2Prefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "N_Birch2");
            MaterialReplacer.RegisterGameObjectForShaderSwap(grownBirch2Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            GameObject birchVFX1Prefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "vfx_birch1_cut1");
            MaterialReplacer.RegisterGameObjectForShaderSwap(birchVFX1Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            GameObject birchVFX2Prefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "vfx_birch2_cut1");
            MaterialReplacer.RegisterGameObjectForShaderSwap(birchVFX2Prefab, MaterialReplacer.ShaderType.UseUnityShader);

            BuildPiece noakSapling = new BuildPiece("flowerbedbundle", "N_Oak_Sapling");
            noakSapling.Name.English("Oak Sapling");
            noakSapling.Description.English("This tree is fur?!");
            noakSapling.RequiredItems.Add("Acorn", 1, false);
            noakSapling.Tool.Add("item_gardentrowel");
            MaterialReplacer.RegisterGameObjectForShaderSwap(noakSapling.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            noakSapling.Prefab.AddComponent<IgnoreGrowSpace>();
            noakSapling.Prefab.AddComponent<RequiresFlowerbed>();
            noakSapling.Prefab.AddComponent<SaplingTracker>();
            GameObject oakTreePrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "N_Oak1");
            MaterialReplacer.RegisterGameObjectForShaderSwap(oakTreePrefab, MaterialReplacer.ShaderType.UseUnityShader);
            GameObject oakVFXPrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "vfx_oak_cut1");
            MaterialReplacer.RegisterGameObjectForShaderSwap(oakVFXPrefab, MaterialReplacer.ShaderType.UseUnityShader);

            BuildPiece raspBush = new BuildPiece("flowerbedbundle", "piece_rasp_bush");
            raspBush.Name.English("Raspberry Bush");
            raspBush.Description.English("Raspberry Bush");
            raspBush.RequiredItems.Add("Raspberry", 5, false);
            raspBush.Tool.Add("item_gardentrowel");
            MaterialReplacer.RegisterGameObjectForShaderSwap(raspBush.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            raspBush.Prefab.AddComponent<RequiresFlowerbed>();

            BuildPiece blueBush = new BuildPiece("flowerbedbundle", "piece_blue_bush");
            blueBush.Name.English("Blueberry Bush");
            blueBush.Description.English("Blue Bush");
            blueBush.RequiredItems.Add("Blueberries", 5, false);
            blueBush.Tool.Add("item_gardentrowel");
            MaterialReplacer.RegisterGameObjectForShaderSwap(blueBush.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            blueBush.Prefab.AddComponent<RequiresFlowerbed>();

            BuildPiece dandeLion1 = new BuildPiece("flowerbedbundle", "N_Dandelion");
            dandeLion1.Name.English("Dandelion");
            dandeLion1.Description.English("An annoying weed.");
            dandeLion1.RequiredItems.Add("Dandelion", 1, false);
            dandeLion1.Tool.Add("item_gardentrowel");
            MaterialReplacer.RegisterGameObjectForShaderSwap(dandeLion1.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            dandeLion1.Prefab.AddComponent<IgnoreGrowSpace>();
            GameObject dandePrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "N_Pickable_Dandelion");
            MaterialReplacer.RegisterGameObjectForShaderSwap(dandePrefab, MaterialReplacer.ShaderType.UseUnityShader);

            BuildPiece redMush = new BuildPiece("flowerbedbundle", "N_Mushroom");
            redMush.Name.English("Mushroom");
            redMush.Description.English("The red Shroom");
            redMush.RequiredItems.Add("Mushroom", 1, false);
            redMush.Tool.Add("item_gardentrowel");
            MaterialReplacer.RegisterGameObjectForShaderSwap(redMush.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            redMush.Prefab.AddComponent<IgnoreGrowSpace>();
            GameObject mushPrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "N_Pickable_Mushroom");
            MaterialReplacer.RegisterGameObjectForShaderSwap(mushPrefab, MaterialReplacer.ShaderType.UseUnityShader);

            BuildPiece yellowMush = new BuildPiece("flowerbedbundle", "N_MushroomYellow");
            yellowMush.Name.English("Yellow Mushroom");
            yellowMush.Description.English("The yellow Shroom");
            yellowMush.RequiredItems.Add("MushroomYellow", 1, false);
            yellowMush.Tool.Add("item_gardentrowel");
            MaterialReplacer.RegisterGameObjectForShaderSwap(yellowMush.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            yellowMush.Prefab.AddComponent<IgnoreGrowSpace>();
            GameObject yellowmushPrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "N_Pickable_Mushroom_yellow");
            MaterialReplacer.RegisterGameObjectForShaderSwap(yellowmushPrefab, MaterialReplacer.ShaderType.UseUnityShader);

            BuildPiece thistlePlant = new BuildPiece("flowerbedbundle", "N_Thistle");
            thistlePlant.Name.English("Thistle");
            thistlePlant.Description.English("Cool Glowing Weed");
            thistlePlant.RequiredItems.Add("Thistle", 1, false);
            thistlePlant.Tool.Add("item_gardentrowel");
            MaterialReplacer.RegisterGameObjectForShaderSwap(thistlePlant.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            thistlePlant.Prefab.AddComponent<IgnoreGrowSpace>();
            GameObject thistlePrefab = PiecePrefabManager.RegisterPrefab("flowerbedbundle", "N_Pickable_Thistle");
            MaterialReplacer.RegisterGameObjectForShaderSwap(thistlePrefab, MaterialReplacer.ShaderType.UseUnityShader);

            BuildPiece growthWard = new BuildPiece("flowerbedbundle", "ward_of_growth");
            growthWard.Name.English("Ward of Growth");
            growthWard.Description.English("Boost the growth of plants in range of this item.");
            growthWard.RequiredItems.Add("FineWood", 25, true);
            growthWard.RequiredItems.Add("SurtlingCore", 5, true);
            growthWard.RequiredItems.Add("AncientSeed", 3, true);
            growthWard.Crafting.Set(PieceManager.CraftingTable.Workbench);
            MaterialReplacer.RegisterGameObjectForShaderSwap(growthWard.Prefab, MaterialReplacer.ShaderType.UseUnityShader);
            growthWard.Prefab.AddComponent<GrowthAura>();

            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmony.PatchAll(assembly);
            SetupWatcher();
        }

        [HarmonyPatch(typeof(Piece), nameof(Piece.OnPlaced))]
        public class FengShuiSkillPatch
        {
            [UsedImplicitly]
            private static void Postfix(Piece __instance)
            {
                if (!Player.m_localPlayer) return;

                var comfort = __instance.m_comfort;
                if (comfort <= 0) return;


                float comfortXP = comfort * 1.0f; // Adjust multiplier
                Player.m_localPlayer.RaiseSkill("Feng Shui", comfortXP);

                // Store comfort value in ZDO
                var znetView = __instance.GetComponent<ZNetView>();
                if (znetView && znetView.IsOwner())
                {
                    znetView.GetZDO().Set("FengShuiSkill Value", comfort);
                }

                Debug.Log($"[FengShuiSkill] Gained {comfortXP} XP from placing comfort item: {__instance.m_name} (Comfort {comfort})");
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.GetComfortLevel))]
        public class LevelScalingPatch
        {
            [UsedImplicitly]
            private static void Postfix(Player __instance, ref int __result)
            {
                if (__instance != Player.m_localPlayer) return;

                float skillFactor = __instance.GetSkillFactor("Feng Shui");

                float comfortBonus = skillFactor * 10f;

                __result += Mathf.FloorToInt(comfortBonus);

                UnityEngine.Debug.Log($"[FengShuiSkill] Comfort skill factor: {skillFactor}, bonus comfort: {comfortBonus}, total comfort: {__result}");
            }
        }

        private void OnDestroy()
        {
            Config.Save();
        }

        private void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                TrowelModLogger.LogDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                TrowelModLogger.LogError($"There was an issue loading your {ConfigFileName}");
                TrowelModLogger.LogError("Please check your config entries for spelling and format!");
            }
        }


        #region ConfigOptions

        private static ConfigEntry<Toggle> _serverConfigLocked = null!;
        private static ConfigEntry<Toggle> _recipeIsActiveConfig = null!;

        private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription =
                new(
                    description.Description +
                    (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                    description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);
            //var configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        private ConfigEntry<T> config<T>(string group, string name, T value, string description,
            bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }

        private class ConfigurationManagerAttributes
        {
            [UsedImplicitly] public int? Order;
            [UsedImplicitly] public bool? Browsable;
            [UsedImplicitly] public string? Category;
            [UsedImplicitly] public Action<ConfigEntryBase>? CustomDrawer;
        }

        #endregion
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.HaveGrowSpace))]
    public class Flowerbed_HaveGrowSpace_Patch
    {
        static void Postfix(Plant __instance, ref bool __result)
        {
            if (__result) return; // Already has space, do nothing

            if (IsOnFlowerbed(__instance.transform.position))
            {
                __result = true;
            }
        }
 
        private static bool IsOnFlowerbed(Vector3 position)
        {
            RaycastHit hit;
            Vector3 origin = position + Vector3.up * 0.5f;
            Vector3 direction = Vector3.down;

            if (Physics.Raycast(origin, direction, out hit, 1f, LayerMask.GetMask("piece")))
            {
                if (hit.collider.GetComponentInParent<IsFlowerbed>() != null)
                {
                    return true;
                }
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.PlacePiece))]
    public class FlowerbedPlacementPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Player __instance, Piece piece, Vector3 pos, Quaternion rot, bool doAttack)
        {
            // Check to see what piece is being placed on
            if (Physics.Raycast(pos + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 2f))
            {
                // Check if piece IsFlowerbed
                var targetPiece = hit.collider.GetComponentInParent<Piece>();
                if (targetPiece != null && targetPiece.GetComponent<IsFlowerbed>() != null)
                {
                    return true;
                }

                // Refund if not on Flowerbed
                if (piece.GetComponent<RequiresFlowerbed>() != null)
                {
                    // Refund resources to player
                    if (piece.m_resources != null)
                    {
                        foreach (var requirement in piece.m_resources)
                        {
                            if (requirement.m_resItem != null)
                            {
                                __instance.m_inventory.AddItem(requirement.m_resItem.name, requirement.m_amount, requirement.m_resItem.m_itemData.m_quality, 0, 0, "");
                            }
                        }
                    }
                    __instance.Message(MessageHud.MessageType.Center, "This plant requires a flowerbed to be placed.");
                    return false;
                }
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.UpdatePlacementGhost))]
    public class PlacementGhostColorPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            var ghost = __instance.m_placementGhost;
            if (ghost == null) return;

            var piece = ghost.GetComponent<Piece>();
            if (piece == null) return;

            if (piece.GetComponent<RequiresFlowerbed>() != null)
            {
                bool validPlacement = false;

                if (Physics.Raycast(ghost.transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 2f))
                {
                    var targetPiece = hit.collider.GetComponentInParent<Piece>();
                    if (targetPiece != null && targetPiece.GetComponent<IsFlowerbed>() != null)
                    {
                        validPlacement = true;
                    }
                }

                SetGhostColor(ghost, validPlacement ? Color.white : Color.red);
            }
            else
            {
                SetGhostColor(ghost, Color.white);
            }
        }

        private static void SetGhostColor(GameObject ghost, Color color)
        {
            foreach (var renderer in ghost.GetComponentsInChildren<Renderer>())
            {
                foreach (var mat in renderer.materials)
                {
                    if (mat.HasProperty("_Color"))
                    {
                        mat.color = color;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.SUpdate))]
    public class GrowthAuraPatch
    {
        private const float GROWTH_REDUCTION_PERCENT = 0.10f;
        private const string GrowthAuraAppliedKey = "growthAuraApplied";

        public static void Prefix(Plant __instance)
        {
            if (!__instance.m_nview || !__instance.m_nview.IsOwner()) return;

            if (!GrowthAura.IsInGrowthAura(__instance.transform.position)) return;

            var zdo = __instance.m_nview.GetZDO();

            // Checks if growth boost already applied
            if (zdo.GetBool(GrowthAuraAppliedKey)) return;

            float growTime = __instance.m_growTime;
            if (growTime <= 0f) return;

            double timeReduction = growTime * GROWTH_REDUCTION_PERCENT;

            long plantTimeTicks = zdo.GetLong(ZDOVars.s_plantTime, ZNet.instance.GetTime().Ticks);
            DateTime adjustedPlantTime = new DateTime(plantTimeTicks).AddSeconds(-timeReduction);
            zdo.Set(ZDOVars.s_plantTime, adjustedPlantTime.Ticks);

            // Mark as applied so we don’t keep changing it
            zdo.Set(GrowthAuraAppliedKey, true);
        }
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.Grow))]
    public class Plant_Grow_Patch
    {
        static void Postfix(Plant __instance)
        {
            var tracker = __instance.GetComponent<SaplingTracker>();
            if (tracker == null) return;

            Vector3 originalPos = tracker.originalPosition;

            GameObject closest = FindClosestNewlyGrownObject(originalPos);
            if (closest != null)
            {
                closest.transform.position = originalPos + new Vector3(0f, 0.5f, 0f); // Lift it up
            }
        }

        static GameObject FindClosestNewlyGrownObject(Vector3 pos)
        {
            float searchRadius = 5f;
            float closestDist = float.MaxValue;
            GameObject closest = null;

            foreach (var go in GameObject.FindObjectsOfType<GameObject>())
            {
                if (go.transform.parent != null) continue;

                float dist = Vector3.Distance(go.transform.position, pos);
                if (dist < searchRadius && dist < closestDist)
                {
                    closestDist = dist;
                    closest = go;
                }
            }

            return closest;
        }
    }

    public static class KeyboardExtensions
    {
        public static bool IsKeyDown(this KeyboardShortcut shortcut)
        {
            return shortcut.MainKey != KeyCode.None && Input.GetKeyDown(shortcut.MainKey) && shortcut.Modifiers.All(Input.GetKey);
        }

        public static bool IsKeyHeld(this KeyboardShortcut shortcut)
        {
            return shortcut.MainKey != KeyCode.None && Input.GetKey(shortcut.MainKey) && shortcut.Modifiers.All(Input.GetKey);
        }
    }
}
