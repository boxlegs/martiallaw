using BlackMagicAPI.Helpers;
using BlackMagicAPI.Modules.Spells;
using BlackMagicAPI.Network;
using System.Collections;
using BlackMagicAPI.Interfaces;
using System.Reflection;
using FishUtilities.Attributes;
using UnityEngine;
using System;
using System.IO;

namespace MartialLawSpell;

// This is all ripped from 
internal class MartialLawSpellLogic : SpellLogic
{
    private static bool active; // Tracks if this spell is currently active (static so only one instance can be active at a time)
    private AudioClip? clip; // Audio clip for the spell effect (static to load once and share across instances)

    // Configuration parameters for the spell
    private static readonly float duration = 20f;        // How long the spell lasts in seconds
    private static readonly float rangeWidth = 50f;      // Width of the area where fireballs can spawn
    private static readonly float rangeDistance = 35f;   // Distance range for fireball spawning

    private static FireballController? firePrefab;

    public override bool CastSpell(PlayerMovement caster, PageController page, Vector3 spawnPos, Vector3 viewDirectionVector, int castingLevel)
    {



        // clip = Assembly.GetExecutingAssembly().LoadWavFromResources($"MartialLawSpell.Resources.Sounds.sound{UnityEngine.Random.RandomRangeInt(1, 3)}.wav");
        PlayClip();

        // Only execute on the owner client to avoid duplicate execution
        if (caster.IsOwner)
        {
            // Send command to start the hellfire routine on host client
            CmdHellfireRoutine(caster.gameObject, viewDirectionVector);

            // Start coroutine to dispose of the spell after it completes
            StartCoroutine(CoWaitDisposeSpell());
        }

        return true;
    }

    private void PlayClip()
    {
        // Create and configure audio source for the spell sound
        var source = gameObject.AddComponent<AudioSource>();
        clip = Assembly.GetExecutingAssembly().LoadWavFromResources($"MartialLawSpell.Resources.Sounds.MartialLaw.wav");
        source.clip = clip;
        source.spatialBlend = 0.4f; // 2D sound (not spatialized)
        source.volume = 0.8f; // Moderate volume
        source.playOnAwake = false; // Don't play automatically
        source.loop = false; // Play once
        source.minDistance = 0.01f; // Very close minimum distance
        source.maxDistance = 1000f; // Very far maximum distance
        source.rolloffMode = AudioRolloffMode.Linear; // Linear volume dropoff
        source.pitch = 1f; // Slightly lowered pitch for eerie effect
        source.Play(); // Start playback

    }

    // Coroutine to wait for spell duration and then dispose of it
    private IEnumerator CoWaitDisposeSpell()
    {
        // Wait for spell duration plus buffer time
        yield return new WaitForSeconds(duration + 5f);

        // Clean up the spell
        DisposeSpell();
        yield break;
    }

    // Command method (executed on host client) to start the hellfire routine
    [FishCmd]
    private static void CmdHellfireRoutine(GameObject caster, Vector3 viewDirectionVector)
    {
        // Start the hellfire coroutine on the caster's PlayerMovement component
        caster.GetComponent<PlayerMovement>().StartCoroutine(CoHellfire(caster, viewDirectionVector * 50f));
    }

    // Main hellfire coroutine that spawns fireballs over time
    private static IEnumerator CoHellfire(GameObject caster, Vector3 viewDirectionVector)
    {
        // Calculate when the spell should end
        float endTime = Time.time + duration;

        // Calculate the center point in front of the player
        Vector3 centerPoint = caster.transform.position + (viewDirectionVector.normalized * (rangeDistance / 2f));
        centerPoint.y = 0f; // Ensure center is at ground level

        // Continue spawning fireballs until time runs out
        while (Time.time < endTime)
        {
            // Generate random angle and distance for circular distribution
            float randomAngle = UnityEngine.Random.Range(0f, 360f);
            float randomDistance = UnityEngine.Random.Range(0f, rangeWidth / 2f); // Use half rangeWidth as radius

            // Convert polar coordinates (angle + distance) to Cartesian coordinates
            Vector3 randomOffset = new(
                Mathf.Cos(randomAngle * Mathf.Deg2Rad) * randomDistance,
                0f,
                Mathf.Sin(randomAngle * Mathf.Deg2Rad) * randomDistance
            );

            // Apply the circular offset relative to the center point
            Vector3 groundTarget = centerPoint + randomOffset;

            // Calculate spawn position above the target (directly overhead)
            Vector3 spawnPosition = groundTarget + new Vector3(0f, 125f, 0f);

            // Calculate direction from spawn position to ground target (straight down)
            Vector3 angledDirection = (groundTarget - spawnPosition).normalized;

            // Spawn fireball on all clients
            RpcSpawnFireball(caster, spawnPosition, angledDirection);

            // Wait random interval before spawning next fireball
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.4f, 0.55f));
        }

        yield break;
    }

    // RPC method (executed on all clients) to spawn individual fireballs
    [FishRpc]
    private static void RpcSpawnFireball(GameObject caster, Vector3 spawnPos, Vector3 direction)
    {
        // Instantiate the fireball prefab
        var fireBall = Instantiate(firePrefab);

        if (fireBall != null)
        {
            // Set fireball properties
            fireBall.transform.position = spawnPos;
            fireBall.playerOwner = caster;

            // Apply initial force to the fireball
            fireBall.rb.AddForce(direction * 75f, ForceMode.VelocityChange);
            fireBall.StartCoroutine("Shoott");
        }
    }

    public override void OnPrefabCreatedAutomatically(GameObject prefab)
    {
        // Find and store reference to the fireball prefab
        firePrefab = Resources.FindObjectsOfTypeAll<FireballController>()[1];
    }
}