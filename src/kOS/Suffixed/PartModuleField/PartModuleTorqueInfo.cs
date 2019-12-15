using kOS.Safe.Encapsulation;

namespace kOS.Suffixed.PartModuleField
{
    using Part;
    using Safe.Encapsulation.Suffixes;
    using UnityEngine;

    /// <summary>
    /// Get torque information from a part module
    /// </summary>
    [kOS.Safe.Utilities.KOSNomenclature("PartModuleTorqueInfo")]
    public class PartModuleTorqueInfo : Structure
    {
        protected readonly PartModule partModule;
        protected readonly ITorqueProvider torqueProvider;
        protected readonly SharedObjects shared;


        /// <summary>
        /// Create a kOS-user variable wrapper around a KSP PartModule attached to a part.
        /// </summary>
        /// <param name="partModule">the KSP PartModule to make a wrapper for</param>
        /// <param name="shared">The omnipresent shared data</param>
        public PartModuleTorqueInfo(PartModule partModule, SharedObjects shared)
        {
            this.partModule = partModule;
            this.torqueProvider = this.partModule as ITorqueProvider;
            this.shared = shared;

            // Overriding Structure.InitializeSuffixes() doesn't work because the base constructor calls it
            // prior to calling this constructor, and so partModule isn't set yet:
            InitializeSuffixesAfterConstruction();
        }

        private void InitializeSuffixesAfterConstruction()
        {
            AddSuffix("PARTMODULE", new Suffix<PartModuleFields>(() => PartModuleFieldsFactory.Construct(partModule, shared)));
            AddSuffix("AVAILABLETORQUE", new Suffix<ListValue>(GetAvailableTorque));
        }

        protected ListValue GetAvailableTorque()
        {
            if (torqueProvider == null)
            {
                return new ListValue(new[]
                {
                    new Vector(0,0,0),
                    new Vector(0,0,0)
                });    
            }

            Vector3 pos, neg;
            torqueProvider.GetPotentialTorque(out pos, out neg);

            return new ListValue(new []
            {
                new Vector(pos),
                new Vector(neg), 
            });
        }
    }
}
