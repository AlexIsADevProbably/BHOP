# BHOP

BHOP is a SCP: Secret Laboratory LabApi plugin for a form of movement in SCP:SL

## Requirements
  - `HintServiceMeow-LabAPI`
  - `AudioPlayerApi`
  - publicized SCP:SL assemblies

## Audio Setup

The plugin searches under the LabAPI plugins directory for a folder named `sounds` that contains `bhop.ogg`.

If you do not have sounds, the plugin should still work.

## Troubleshooting

- `Failed to load BHOP custom audio clips`
  - Check that the `sounds` folder exists somewhere under the LabAPI plugins directory.
  - Make sure `bhop.ogg`, `aura.ogg`, and `boing.ogg` are present.

- Build warning `NU1504: Duplicate 'PackageReference'`
  - The project currently references `BepInEx.AssemblyPublicizer.MSBuild` twice with different versions. The build can still succeed, but the duplicate should be cleaned up later.

## License

See [LICENSE](LICENSE).
