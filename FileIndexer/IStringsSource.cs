﻿namespace FileIndexer
{
    public interface IStringsSource
    {
        string ReadString(long start, long end, StringSymbolsFilter symbolsFilter = null);
    }
}