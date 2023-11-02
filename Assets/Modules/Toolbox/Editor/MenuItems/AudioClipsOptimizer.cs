using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Optimizes all AudioClips' import settings to the best settings according to:
/// https://gamedevbeginner.com/unity-audio-optimisation-tips/
/// </summary>
public static class AudioClipsOptimizer
{
    #region Main
    public const float MIDDLE_LENGTH = 3f; // Longer SFX
    public const float LONG_LENGTH = 10f; // Music, Ambiance, etc
    public const int QUALITY = 70;

    private static readonly string CLIP_SEARCH = $"t:{typeof(AudioClip).Name}";

    [MenuItem("Tools/Ble/Assets/Optimize AudioClips")]
    public static void OptimizeAudioClips()
    {
        bool isYes = EditorUtility.DisplayDialog("Are you sure?",
            "This will reimport ALL AudioClips and might take a while.",
            "Yes", "No");

        if (false == isYes)
            return;

        FetchAssets();
        OptimizeAll();
    }

    private static void FetchAssets()
    {
        IEnumerable<string> paths = AssetDatabase.FindAssets(CLIP_SEARCH)
            .Select(g => AssetDatabase.GUIDToAssetPath(g));

        Clips = paths.Select(p => AssetDatabase.LoadAssetAtPath<AudioClip>(p))
            .ToArray();
        Importers = paths.Select(p => AssetImporter.GetAtPath(p) as AudioImporter)
            .ToArray();
    }

    private static AudioClip[] Clips { get; set; }
    private static AudioImporter[] Importers { get; set; }
    #endregion



    #region Optimization
    private static void OptimizeAll()
    {
        for (int i = 0; i < Importers.Length; i++)
        {
            bool isCanceled = EditorUtility.DisplayCancelableProgressBar(
                "Optimizing AudioClips...",
                Clips[i].name,
                (float)i / Importers.Length);

            if (isCanceled)
                break;

            try
            {
                OptimizeAt(i);
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                throw e;
            }
        }
        EditorUtility.ClearProgressBar();
    }

    private static void OptimizeAt(int index)
    {
        AudioImporter imp = Importers[index];
        AudioClip clip = Clips[index];

        TryOptimizeForceMono(imp, clip);
        OptimizePlatformSettings(imp, clip);

        imp.loadInBackground = true;
        if (EditorUtility.IsDirty(imp))
            imp.SaveAndReimport();
    }

    private static void TryOptimizeForceMono(AudioImporter importer, AudioClip clip)
    {
        importer.forceToMono = Is3D(clip);
        if (importer.forceToMono)
        {
            SerializedObject serializedObject = new SerializedObject(importer);
            SerializedProperty normalize = serializedObject.FindProperty("m_Normalize");
            normalize.boolValue = false;
            serializedObject.ApplyModifiedProperties();
        }
    }

    private static void OptimizePlatformSettings(AudioImporter importer, AudioClip clip)
    {
        AudioImporterSampleSettings samples = importer.defaultSampleSettings;
        if (clip.length >= LONG_LENGTH)
        {
            samples.loadType = AudioClipLoadType.Streaming;
            samples.compressionFormat = AudioCompressionFormat.ADPCM;
        }
        else if (clip.length >= MIDDLE_LENGTH)
        {
            samples.loadType = AudioClipLoadType.CompressedInMemory;
            samples.compressionFormat = AudioCompressionFormat.Vorbis;
            samples.quality = QUALITY;
        }
        else
        {
            samples.loadType = AudioClipLoadType.DecompressOnLoad;
            samples.compressionFormat = AudioCompressionFormat.Vorbis;
            samples.quality = QUALITY;
        }
        samples.sampleRateSetting = AudioSampleRateSetting.OptimizeSampleRate;
        importer.defaultSampleSettings = samples;
    }

    private static bool Is3D(AudioClip clip)
    {
        //Todo: Dependent on the Audio System!
        return false;
    }
    #endregion
}