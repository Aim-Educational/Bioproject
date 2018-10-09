#ifndef UTIL_H
#define UTIL_H

#define htons(x) ( ((x)<< 8 & 0xFF00) | \
                   ((x)>> 8 & 0x00FF) )
#define ntohs(x) htons(x)

#define htonl(x) ( ((x)<<24 & 0xFF000000UL) | \
                   ((x)<< 8 & 0x00FF0000UL) | \
                   ((x)>> 8 & 0x0000FF00UL) | \
                   ((x)>>24 & 0x000000FFUL) )
#define ntohl(x) htonl(x)

#define CREATE_ENUM_CLASS_OPERATOR_OVERLOADS(enumName)                          \
  inline enumName operator | (enumName lhs, enumName rhs)                       \
  {                                                                             \
    return (enumName)(static_cast<enumName>(lhs) | static_cast<enumName>(rhs)); \
  }                                                                             \
  inline enumName operator & (enumName lhs, enumName rhs)                       \
  {                                                                             \
    return (enumName)(static_cast<enumName>(lhs) & static_cast<enumName>(rhs)); \
  }                                                                             \
  inline enumName& operator |= (enumName& lhs, enumName rhs)                    \
  {                                                                             \
    lhs = (enumName)(static_cast<enumName>(lhs) | static_cast<enumName>(rhs));  \
    return lhs;                                                                 \
  }                                                                             \
  inline enumName& operator &= (enumName& lhs, enumName rhs)                    \
  {                                                                             \
    lhs = (enumName)(static_cast<enumName>(lhs) & static_cast<enumName>(rhs));  \
    return lhs;                                                                 \                                                       
  }      

#endif
