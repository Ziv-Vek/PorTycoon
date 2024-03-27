char* swStringCopy(const char* string)
{
    if (string == NULL)
    return NULL;
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}
