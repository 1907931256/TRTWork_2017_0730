#import matplotlib.pyplot as plt
def show():
    x = np.linspace(0, 1, 500)
    y = np.sin(4 * np.pi * x) * np.exp(-5 * x)

    fig, ax = plt.subplots()

    ax.fill(x, y, zorder=10)
    ax.grid(True, zorder=5)
    plt.show()
def add(num1,num2):
    return num1+num2
def get_sys_path():
    import sys
    return ';'.join(sys.path)
