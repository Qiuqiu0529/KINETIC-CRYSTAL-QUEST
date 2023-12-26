import numpy as np
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D

def pca(data, num_components):
    # 1. 计算数据的协方差矩阵
    cov_matrix = np.cov(data.T)

    # 2. 计算协方差矩阵的特征向量和特征值
    eigenvalues, eigenvectors = np.linalg.eig(cov_matrix)

    # 3. 选择主成分
    sorted_indices = np.argsort(eigenvalues)[::-1]
    top_indices = sorted_indices[:num_components]
    top_eigenvectors = eigenvectors[:, top_indices]

    # 4. 投影到新的低维空间
    reduced_data = np.dot(data, top_eigenvectors)

    return reduced_data

# 从文件读取数据并预处理
data_file = "masterAction.txt"

with open(data_file, 'r') as f:
    lines = f.readlines()

data = []
current_matrix = []
for line in lines:
    if line.strip():  # 非空行
        values = [float(val) for val in line.split()]
        current_matrix.append(values)
    else:  # 空行，表示矩阵结束
        if current_matrix:
            data.append(np.array(current_matrix))
            current_matrix = []

if current_matrix:
    data.append(np.array(current_matrix))

data_sequence = np.array(data)

# 调用 PCA 函数，降低维度为3
reduced_data_sequence = []
for matrix in data_sequence:
    reduced_matrix = pca(matrix, num_components=3)
    reduced_data_sequence.append(reduced_matrix)

reduced_data_sequence = np.array(reduced_data_sequence)

# 可视化降维后的数据
fig = plt.figure()
ax = fig.add_subplot(111, projection='3d')

# 将所有矩阵的数据按顺序连接成一条线
for matrix in reduced_data_sequence:
    ax.plot(matrix[:, 0], matrix[:, 1], matrix[:, 2])

ax.set_xlabel('Principal Component 1')
ax.set_ylabel('Principal Component 2')
ax.set_zlabel('Principal Component 3')

plt.show()
