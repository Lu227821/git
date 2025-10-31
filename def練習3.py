
def org(z,a):
    z=f"{z:0{a}b}"
    return list(map(int,z)) #利用map來讓list轉字元
print(org(8,8))

def orb(b):
    #b=f"{b:016b}"
    return int("".join(map(str,b)),2) #語法int(x,base)誰轉某進制 ," ".join(x) 某字串拼成一個大字串
print(orb([0,0,0,0,1,0,0,0]))

def orb(b):
    result = 0
    for bit in b:
        result = (result << 1) | bit   # 左移一位，加上當前 bit
    return result
print(orb([0,0,0,0,1,0,0,0]))

def orc(c,s):
    c=f"{c:0{s}b}"
    return (c)
print(orc(8,8))
#-----------------

