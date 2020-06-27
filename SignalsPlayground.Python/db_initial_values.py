import math
import numpy as np
import scipy.optimize as optimize
from wavelet_coefficients import get_scaling_coefficients

# Wavelets from db2 to db25
wavelet_name = 'db4'

# Helper method for minimization: ||Ax||^2
def f(x):
    y = np.dot(A, x)
    return np.dot(y, y)

# maps 0 or coefficient values to matrix A
def map_index(b, i, j):
    index = 2 * i - j
    if index < 0 or index >= len(b):
        return 0
    else:
        return b[index]

scaling_coef = np.array(get_scaling_coefficients(wavelet_name))
bVector = scaling_coef / (1 / scaling_coef.sum()) # normalize coefficients

N = len(bVector)
A_list = [[map_index(bVector, row, col) for col in range(1, N-1)] for row in range(1, N-1)]
A = np.array(A_list) - np.identity(N - 2)
print(f'A: {A}\n')

zeroVector = np.array(np.zeros(N - 2))

cons = ({'type': 'eq', 'fun': lambda x: x.sum() - 1})
res = optimize.minimize(f, zeroVector, method='SLSQP', constraints=cons, options={'disp' : False})

xbest = res['x']
print(f'{wavelet_name} initial values: {xbest}\n')
print(f'sum of initial values: {xbest.sum()}')