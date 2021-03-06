using System.Collections.Generic;
using System.Linq;
using EDEngineer.Models.Utils.Json;

namespace EDEngineer.Models.Operations
{
    public class DumpOperation : JournalOperation
    {
        public List<MaterialOperation> DumpOperations { get; set; } 

        public HashSet<Kind> ResetFilter { get; set; }

        public override void Mutate(State state)
        {
            var dump = DumpOperations.ToDictionary(m => m.MaterialName, m => m.Size);
            foreach (var item in state.Cargo.Where(item => ResetFilter.Contains(item.Value.Data.Kind)).ToList())
            {
                var currentValue = item.Value.Count;

                if (dump.TryGetValue(item.Key, out var toSetValue))
                {
                    if (currentValue != toSetValue)
                    {
                        state.IncrementCargo(item.Key, toSetValue - currentValue);
                    }
                }
                else if (currentValue != 0)
                {
                    state.IncrementCargo(item.Key, -1 * currentValue);
                }
            }

            var names = state.Cargo.Keys.ToHashSet();

            foreach (var item in DumpOperations.Where(op => !names.Contains(op.MaterialName)))
            {
                state.IncrementCargo(item.MaterialName, item.Size);
            }
        }
    }
}