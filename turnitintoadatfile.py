file = '''
ctx.moveTo(160.000000, -120.000000);
ctx.lineTo(160.000000, -600.000000);
ctx.lineTo(480.000000, -840.000000);
ctx.lineTo(800.000000, -600.000000);
ctx.lineTo(800.000000, -120.000000);
ctx.lineTo(560.000000, -120.000000);
ctx.lineTo(560.000000, -400.000000);
ctx.lineTo(400.000000, -400.000000);
ctx.lineTo(400.000000, -120.000000);
ctx.lineTo(160.000000, -120.000000);
'''
final = []
for section in file.strip().split("\n"):
    beautifulstring = (section.strip()[section.strip().find("(")+1:-2])
    splitstring = beautifulstring.split(", ")
    splitstring = [int(i.replace(".000000", "")) for i in splitstring]
    splitstring[1] += 480
    splitstring = [str(round((i / 960), 3)) for i in splitstring]
    print(" ".join(splitstring))
    final.append(" \t".join(splitstring))
with open("output.dat", "w") as file:
    file.write("OUTPUT AIRFOIL\n"+("\n".join(final)))
