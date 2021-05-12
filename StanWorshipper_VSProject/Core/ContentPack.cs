using RoR2.ContentManagement;

namespace StanWorshipper.Core
{
    internal class StanWorshipperContentPack : IContentPackProvider
    {
        public ContentPack contentPack = new ContentPack();
        public string identifier => "com.Vodhr.StanWorshipperSurvivor";

        public void Initialize()
        {
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }

        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }

        public System.Collections.IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            // Apparently R2API has it's own content pack and any of the "Add" methods in the various modules also add those assets to
            // R2API's content pack. Much of this was unneeded. Sigh.

            this.contentPack.identifier = this.identifier;
            contentPack.bodyPrefabs.Add(StanWorshipperPlugin.characterBodies.ToArray());
            contentPack.buffDefs.Add(StanWorshipperPlugin.buffDefs.ToArray());
            contentPack.effectDefs.Add(StanWorshipperPlugin.effectDefs.ToArray());
            contentPack.entityStateTypes.Add(StanWorshipperPlugin.entityStateTypes.ToArray());
            contentPack.masterPrefabs.Add(StanWorshipperPlugin.characterMasters.ToArray());
            contentPack.networkSoundEventDefs.Add(StanWorshipperPlugin.networkSoundEventDefs.ToArray());
            contentPack.projectilePrefabs.Add(StanWorshipperPlugin.projectilePrefabs.ToArray());
            contentPack.skillDefs.Add(StanWorshipperPlugin.skillDefs.ToArray());
            contentPack.skillFamilies.Add(StanWorshipperPlugin.skillFamilies.ToArray());
            contentPack.survivorDefs.Add(StanWorshipperPlugin.survivorDefs.ToArray());
            contentPack.unlockableDefs.Add(StanWorshipperPlugin.unlockableDefs.ToArray());

            args.ReportProgress(1f);
            yield break;
        }

        public System.Collections.IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(this.contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public System.Collections.IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
    }
}
