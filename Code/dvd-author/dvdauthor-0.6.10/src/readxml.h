typedef void (*parserfunc)(void);
typedef void (*attrfunc)(char *);

struct elemdesc {
    char *elemname;
    int parentstate,newstate;
    parserfunc start,end;
};

struct elemattr {
    char *elem,*attr;
    attrfunc f;
};

int readxml(const char *my_xml_buffer,struct elemdesc *elems,struct elemattr *attrs);
int xml_ison(const char *v);
char *utf8tolocal(const char *in);

extern int parser_err, parser_acceptbody;
extern char *parser_body;

