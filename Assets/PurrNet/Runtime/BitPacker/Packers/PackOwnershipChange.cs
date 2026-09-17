using PurrNet.Modules;
using PurrNet.Pooling;
using UnityEngine;

namespace PurrNet.Packing
{
    public static class PackOwnerShipChange
    {
        [UsedByIL]
        internal static void WriteOwnershipChange(this BitPacker packer, OwnershipChange value)
        {
            Debug.Log($"Packer hit at {Time.time}");
            Packer<SceneID>.Write(packer, value.sceneId);
            Packer<bool>.Write(packer, value.isAdding);
            Packer<PlayerID>.Write(packer, value.player);
            Packer<bool>.Write(packer, value.isSpawner);

            WriteIdentitiesRLE(packer, value.identities);
        }

        [UsedByIL]
        internal static void ReadOwnershipChange(this BitPacker packer, ref OwnershipChange value)
        {
            Packer<SceneID>.Read(packer, ref value.sceneId);
            Packer<bool>.Read(packer, ref value.isAdding);
            Packer<PlayerID>.Read(packer, ref value.player);
            Packer<bool>.Read(packer, ref value.isSpawner);

            ReadIdentitiesRLE(packer, ref value.identities);
        }

        private static void WriteIdentitiesRLE(BitPacker packer, DisposableList<NetworkID> identities)
        {
            if (identities.isDisposed || identities.rawList == null)
            {
                packer.WriteBit(false);
                return;
            }

            packer.WriteBit(true);

            int count = identities.Count;
            Packer<Size>.Write(packer, (uint)count);

            int i = 0;
            while (i < count)
            {
                var runStart = identities[i];
                int runLength = 1;

                while (i + runLength < count)
                {
                    var prev = identities[i + runLength - 1];
                    var next = identities[i + runLength];

                    bool sameScope = prev.scope.Equals(next.scope) && prev.scope.isBot == next.scope.isBot;
                    bool consecutive = next.id.value == prev.id.value + 1;

                    if (!sameScope || !consecutive)
                        break;

                    runLength++;
                }

                Packer<PlayerID>.Write(packer, runStart.scope);
                Packer<PackedULong>.Write(packer, runStart.id);
                Packer<Size>.Write(packer, (uint)runLength);

                i += runLength;
            }
        }

        static void ReadIdentitiesRLE(BitPacker packer, ref DisposableList<NetworkID> identities)
        {
            identities.Dispose();

            bool hasValue = default;
            packer.Read(ref hasValue);

            if (!hasValue)
                return;

            Size totalCount = default;
            Packer<Size>.Read(packer, ref totalCount);

            identities = DisposableList<NetworkID>.Create(totalCount);

            int read = 0;
            while (read < totalCount)
            {
                PlayerID scope = default;
                Packer<PlayerID>.Read(packer, ref scope);

                PackedULong startId = default;
                Packer<PackedULong>.Read(packer, ref startId);

                Size runLength = default;
                Packer<Size>.Read(packer, ref runLength);

                for (int j = 0; j < runLength; j++)
                    identities.Add(new NetworkID(startId.value + (ulong)j, scope));

                read += runLength;
            }
        }
    }
}
