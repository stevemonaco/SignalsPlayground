import math
import numpy as np
import scipy.optimize as optimize
from wavelet_coefficients import get_scaling_coefficients

# Wavelets from db2 to db25
wavelet_name = 'db7'

def f(x):
    y = np.dot(A, x) - b
    return np.dot(y, y)

def map_index(h, i, j):
    index = 2 * i - j
    if index < 0 or index >= len(h):
        return 0
    else:
        return h[index]

scaling_coef = np.array(get_scaling_coefficients(wavelet_name))
h = scaling_coef / (1 / scaling_coef.sum())

N = len(h) // 2
A_list = [[map_index(h, row, col) for col in range(1, 2*N-1)] for row in range(1, 2*N-1)]
A = np.array(A_list) - np.identity(2*N - 2)
print(f'A: {A}\n')

b = np.array(np.zeros(2*N - 2))

cons = ({'type': 'eq', 'fun': lambda x: x.sum() - 1})
res = optimize.minimize(f, b, method='SLSQP', constraints=cons, options={'disp' : False})

xbest = res['x']
print(f'{wavelet_name} initial values: {xbest}\n')
print(f'sum of initial values: {xbest.sum()}')