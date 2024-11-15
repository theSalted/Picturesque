using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System;
using System.Collections.Generic;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }

    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private List<AnimatorBinding> animatorBindings = new List<AnimatorBinding>();
    
    [Serializable]
    public class AnimatorBinding
    {
        public string identifier;
        public Animator animator;
    }
    
    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"Multiple CutsceneManager instances detected. Destroying duplicate on {gameObject.name}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // Optional: Make the CutsceneManager persist between scenes
        // DontDestroyOnLoad(gameObject);

        // Initialize components
        if (playableDirector == null)
        {
            playableDirector = GetComponent<PlayableDirector>();
        }
        
        // Validate animator bindings
        for (int i = animatorBindings.Count - 1; i >= 0; i--)
        {
            if (animatorBindings[i].animator == null)
            {
                Debug.LogWarning($"Null animator found in binding {i}. Removing binding.");
                animatorBindings.RemoveAt(i);
            }
        }
    }

    private void OnDestroy()
    {
        // Clean up singleton reference when destroyed
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// Play timeline with specified animator by index
    /// </summary>
    /// <param name="index">Index of the animator in the bindings list</param>
    /// <returns>True if successfully played, false if index was invalid</returns>
    public bool PlayAnimatorByIndex(int index)
    {
        if (index < 0 || index >= animatorBindings.Count)
        {
            Debug.LogError($"Invalid animator index: {index}. Valid range is 0-{animatorBindings.Count - 1}");
            return false;
        }
        
        return PlayAnimatorInternal(animatorBindings[index].animator);
    }
    
    /// <summary>
    /// Play timeline with specified animator by identifier
    /// </summary>
    /// <param name="identifier">Identifier string of the animator binding</param>
    /// <returns>True if successfully played, false if identifier wasn't found</returns>
    public bool PlayAnimatorByIdentifier(string identifier)
    {
        var binding = animatorBindings.Find(x => x.identifier == identifier);
        if (binding == null)
        {
            Debug.LogError($"No animator binding found with identifier: {identifier}");
            return false;
        }
        
        return PlayAnimatorInternal(binding.animator);
    }
    
    /// <summary>
    /// Get all available animator identifiers
    /// </summary>
    /// <returns>Array of animator binding identifiers</returns>
    public string[] GetAvailableAnimators()
    {
        return animatorBindings.ConvertAll(x => x.identifier).ToArray();
    }
    
    private bool PlayAnimatorInternal(Animator selectedAnimator)
    {
        TimelineAsset timeline = playableDirector.playableAsset as TimelineAsset;
        
        if (timeline == null)
        {
            Debug.LogError("No Timeline asset assigned to PlayableDirector");
            return false;
        }
        
        // Bind the selected animator to all animation tracks
        foreach (var track in timeline.GetOutputTracks())
        {
            if (track is AnimationTrack animTrack)
            {
                playableDirector.SetGenericBinding(track, selectedAnimator);
            }
        }
        
        playableDirector.Play();
        return true;
    }

    // Editor validation
    private void OnValidate()
    {
        // Ensure all bindings have unique identifiers
        HashSet<string> usedIdentifiers = new HashSet<string>();
        for (int i = 0; i < animatorBindings.Count; i++)
        {
            string baseIdentifier = string.IsNullOrEmpty(animatorBindings[i].identifier) 
                ? $"Animator{i}" 
                : animatorBindings[i].identifier;
                
            string uniqueIdentifier = baseIdentifier;
            int counter = 1;
            
            while (usedIdentifiers.Contains(uniqueIdentifier))
            {
                uniqueIdentifier = $"{baseIdentifier}_{counter}";
                counter++;
            }
            
            animatorBindings[i].identifier = uniqueIdentifier;
            usedIdentifiers.Add(uniqueIdentifier);
        }
    }
}