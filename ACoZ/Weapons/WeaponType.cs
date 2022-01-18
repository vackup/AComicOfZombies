namespace ACoZ.Weapons
{
    /// <summary>
    /// Tipos de armas / armas que existen en el juego. Si se agrega un nuevo tipo de arma, debe incrementarse la constante GlobalParameters.MAX_WEAPONS_AVAILABLE
    /// </summary>
    public enum WeaponType
    {
        // Primary
        PistolaColtPython = 3,
        PistolaColtM1911 = 2,
        PistolaBerettaM9 = 1,

        // Secondary
        EscopetaBeretta682 = 4,
        EscopetaIthaca37 = 5,
        EscopetaSpas12 = 6,

        // Primary
        SubFusilUzi = 9,
        SubFusilMp5K = 7,
        SubFusilMp40 = 8,

        // Secondary
        FusilAk47 = 10,
        FusilM4A1 = 11,
        FusilXm8 = 12,
    }
}