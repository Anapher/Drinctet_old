(Get-Content src/Drinctet.Core/Parsing/TextDecoder/SpanTextDecoder.cs).replace('FEATURE_SPAN', '!FEATURE_SPAN').replace(".AsSpan()", "").replace(".Slice", ".Substring").replace("ReadOnlySpan<char>", "string").replace(".ToString()", "") | Set-Content src/Drinctet.Core/Parsing/TextDecoder/StringTextDecoder.cs