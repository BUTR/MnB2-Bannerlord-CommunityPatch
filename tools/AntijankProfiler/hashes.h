#include <utility>
#include <functional>


struct insensitive_wstring_hash {
    inline std::size_t operator()(const std::wstring &str) const {
        size_t hash = 0;
        for(wchar_t wc : str)
            hash = ( hash ^ (unsigned) std::toupper(wc) ) * 0x100010001u;
        return hash;
    }
};
struct insensitive_wstring_compare {
    inline bool operator()(const std::wstring &a, const std::wstring &b) const {
        return _wcsicmp(a.c_str(), b.c_str()) == 0;
    }
};
