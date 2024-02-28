using Mutagen.Bethesda.Plugins;

namespace SRExteriorCitiesPatcher
{
    internal class NavmeshMapping
    {
        private readonly Dictionary<FormKey, FormKey> navmeshMap = new();

        public NavmeshMapping()
        {
            navmeshMap.TryAdd(FormKey.Factory("108D5B:Skyrim.esm"), FormKey.Factory("00080C:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("05A18E:Skyrim.esm"), FormKey.Factory("00080D:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("05A193:Skyrim.esm"), FormKey.Factory("00080E:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("05A196:Skyrim.esm"), FormKey.Factory("00080F:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("05A197:Skyrim.esm"), FormKey.Factory("000810:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("10FB3C:Skyrim.esm"), FormKey.Factory("000811:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("02E50F:Skyrim.esm"), FormKey.Factory("000812:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0ED002:Skyrim.esm"), FormKey.Factory("000813:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0F7797:Skyrim.esm"), FormKey.Factory("000814:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("05A18F:Skyrim.esm"), FormKey.Factory("000815:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0BBCFD:Skyrim.esm"), FormKey.Factory("000816:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0D2A64:Skyrim.esm"), FormKey.Factory("000817:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("02361E:Skyrim.esm"), FormKey.Factory("000818:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("05156D:Skyrim.esm"), FormKey.Factory("000819:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0E6C86:Skyrim.esm"), FormKey.Factory("00081A:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("10FB3A:Skyrim.esm"), FormKey.Factory("00081B:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("10FB3B:Skyrim.esm"), FormKey.Factory("00081C:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("041B96:Skyrim.esm"), FormKey.Factory("00081D:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("10FB38:Skyrim.esm"), FormKey.Factory("00081E:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("10FB39:Skyrim.esm"), FormKey.Factory("00081F:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("041B9B:Skyrim.esm"), FormKey.Factory("000820:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0C8897:Skyrim.esm"), FormKey.Factory("000821:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0664BF:Skyrim.esm"), FormKey.Factory("000822:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0D2996:Skyrim.esm"), FormKey.Factory("000823:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0FEAE7:Skyrim.esm"), FormKey.Factory("000824:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("05156A:Skyrim.esm"), FormKey.Factory("000825:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0FBEF2:Skyrim.esm"), FormKey.Factory("000826:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("105319:Skyrim.esm"), FormKey.Factory("000827:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("041B98:Skyrim.esm"), FormKey.Factory("000828:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("05156B:Skyrim.esm"), FormKey.Factory("000829:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("051574:Skyrim.esm"), FormKey.Factory("00082A:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0941DF:Skyrim.esm"), FormKey.Factory("00082B:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0941E0:Skyrim.esm"), FormKey.Factory("00082C:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("105318:Skyrim.esm"), FormKey.Factory("00082D:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("10531A:Skyrim.esm"), FormKey.Factory("00082E:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("037DE3:Skyrim.esm"), FormKey.Factory("00082F:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("037DE2:Skyrim.esm"), FormKey.Factory("000830:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("051575:Skyrim.esm"), FormKey.Factory("000831:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("037DDB:Skyrim.esm"), FormKey.Factory("000832:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("10812F:Skyrim.esm"), FormKey.Factory("000833:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("098204:Skyrim.esm"), FormKey.Factory("000834:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0ED015:Skyrim.esm"), FormKey.Factory("000835:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0ED022:Skyrim.esm"), FormKey.Factory("000836:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0BE45B:Skyrim.esm"), FormKey.Factory("000837:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("03030C:Skyrim.esm"), FormKey.Factory("000838:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0F1A86:Skyrim.esm"), FormKey.Factory("000839:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("02E50D:Skyrim.esm"), FormKey.Factory("00083A:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0660C6:Skyrim.esm"), FormKey.Factory("00083B:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("10E396:Skyrim.esm"), FormKey.Factory("00083C:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0B8C54:Skyrim.esm"), FormKey.Factory("00083D:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("04229F:Skyrim.esm"), FormKey.Factory("00083E:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("07C3E1:Skyrim.esm"), FormKey.Factory("00083F:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0F33C4:Skyrim.esm"), FormKey.Factory("000840:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0F33C5:Skyrim.esm"), FormKey.Factory("000841:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("104BB0:Skyrim.esm"), FormKey.Factory("000842:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("056B9C:Skyrim.esm"), FormKey.Factory("000843:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0F7E95:Skyrim.esm"), FormKey.Factory("000844:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0422AE:Skyrim.esm"), FormKey.Factory("000845:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0F7E92:Skyrim.esm"), FormKey.Factory("000846:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0F7E93:Skyrim.esm"), FormKey.Factory("000847:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0CD8E0:Skyrim.esm"), FormKey.Factory("000848:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("039A00:Skyrim.esm"), FormKey.Factory("000849:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("05DB72:Skyrim.esm"), FormKey.Factory("00084A:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("09E584:Skyrim.esm"), FormKey.Factory("00084B:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0A2E7E:Skyrim.esm"), FormKey.Factory("00084C:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0F20C5:Skyrim.esm"), FormKey.Factory("00084D:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0F7E96:Skyrim.esm"), FormKey.Factory("00084E:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("018A1A:Skyrim.esm"), FormKey.Factory("00084F:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("018A1B:Skyrim.esm"), FormKey.Factory("000850:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("018A1C:Skyrim.esm"), FormKey.Factory("000851:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("018A26:Skyrim.esm"), FormKey.Factory("000852:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("018A17:Skyrim.esm"), FormKey.Factory("000853:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("018A1E:Skyrim.esm"), FormKey.Factory("000854:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("03D2CE:Skyrim.esm"), FormKey.Factory("000855:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("018A1F:Skyrim.esm"), FormKey.Factory("000856:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("018A27:Skyrim.esm"), FormKey.Factory("000857:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("051518:Skyrim.esm"), FormKey.Factory("000858:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("051519:Skyrim.esm"), FormKey.Factory("000859:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("05FC89:Skyrim.esm"), FormKey.Factory("00085A:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("05FC8A:Skyrim.esm"), FormKey.Factory("00085B:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0B324C:Skyrim.esm"), FormKey.Factory("00085C:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0DD46F:Skyrim.esm"), FormKey.Factory("00085D:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0B3256:Skyrim.esm"), FormKey.Factory("00085E:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0B3264:Skyrim.esm"), FormKey.Factory("00085F:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0B3265:Skyrim.esm"), FormKey.Factory("000860:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0B3288:Skyrim.esm"), FormKey.Factory("000861:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0B324F:Skyrim.esm"), FormKey.Factory("000862:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0B3250:Skyrim.esm"), FormKey.Factory("000863:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0B3252:Skyrim.esm"), FormKey.Factory("000864:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0B3253:Skyrim.esm"), FormKey.Factory("000865:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0B3254:Skyrim.esm"), FormKey.Factory("000866:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0B3263:Skyrim.esm"), FormKey.Factory("000867:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0E0D88:Skyrim.esm"), FormKey.Factory("000868:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0B095A:Skyrim.esm"), FormKey.Factory("000869:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0B095B:Skyrim.esm"), FormKey.Factory("00086A:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0D82C1:Skyrim.esm"), FormKey.Factory("00086B:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0D82C2:Skyrim.esm"), FormKey.Factory("00086C:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("108B7D:Skyrim.esm"), FormKey.Factory("00086D:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("037F78:Skyrim.esm"), FormKey.Factory("00086E:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0F337C:Skyrim.esm"), FormKey.Factory("00086F:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0B325C:Skyrim.esm"), FormKey.Factory("000870:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0B325E:Skyrim.esm"), FormKey.Factory("000871:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0B329C:Skyrim.esm"), FormKey.Factory("000872:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0B329D:Skyrim.esm"), FormKey.Factory("000873:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0C34B8:Skyrim.esm"), FormKey.Factory("000874:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0D82C4:Skyrim.esm"), FormKey.Factory("000875:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0F7190:Skyrim.esm"), FormKey.Factory("000876:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("1093D1:Skyrim.esm"), FormKey.Factory("000877:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0B0944:Skyrim.esm"), FormKey.Factory("000878:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0B0945:Skyrim.esm"), FormKey.Factory("000879:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("037F7B:Skyrim.esm"), FormKey.Factory("00087A:SR Exterior Cities.esp"));
            navmeshMap.TryAdd(FormKey.Factory("0ECFFB:Skyrim.esm"), FormKey.Factory("00087B:SR Exterior Cities.esp"));


        }

        public Dictionary<FormKey, FormKey> NavmeshMap => navmeshMap;

    }
}
